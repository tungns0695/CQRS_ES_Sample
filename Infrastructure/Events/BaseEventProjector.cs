using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Events
{
    public abstract class BaseEventProjector : IEventProjector
    {
        protected readonly IProjectorDbContext _dbContext;
        protected readonly ILogger<BaseEventProjector> _logger;

        protected BaseEventProjector(IProjectorDbContext dbContext, ILogger<BaseEventProjector> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public abstract string ProjectorName { get; }

        public async Task<long> GetLastProcessedPositionAsync()
        {
            var checkpoint = await _dbContext.ProjectorCheckpoints
                .FirstOrDefaultAsync(c => c.ProjectorName == ProjectorName);

            return checkpoint?.LastPosition ?? 0;
        }

        public async Task ProcessEventAsync(IEvent @event, long position)
        {
            // Check if event has already been processed (idempotency)
            if (await HasEventBeenProcessedAsync(@event.Id))
            {
                _logger.LogDebug("Event {EventId} already processed by projector {ProjectorName}, skipping", 
                    @event.Id, ProjectorName);
                return;
            }

            using var transaction = await _dbContext.BeginTransactionAsync();
            
            try
            {
                await MarkEventAsProcessedAsync(@event.Id);

                await ProcessEventInternalAsync(@event, position);

                await UpdateCheckpointAsync(position);

                await transaction.CommitAsync();
                
                _logger.LogDebug("Successfully processed event {EventId} at position {Position} for projector {ProjectorName}", 
                    @event.Id, position, ProjectorName);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to process event {EventId} at position {Position} for projector {ProjectorName}", 
                    @event.Id, position, ProjectorName);
                throw;
            }
        }

        public async Task UpdateCheckpointAsync(long position)
        {
            var checkpoint = await _dbContext.ProjectorCheckpoints
                .FirstOrDefaultAsync(c => c.ProjectorName == ProjectorName);

            if (checkpoint == null)
            {
                checkpoint = new ProjectorCheckpoint
                {
                    ProjectorName = ProjectorName,
                    LastPosition = position,
                    LastUpdated = DateTime.UtcNow
                };
                _dbContext.ProjectorCheckpoints.Add(checkpoint);
            }
            else
            {
                checkpoint.LastPosition = position;
                checkpoint.LastUpdated = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> HasEventBeenProcessedAsync(string eventId)
        {
            return await _dbContext.AppliedEvents
                .AnyAsync(e => e.EventId == eventId && e.ProjectorName == ProjectorName);
        }

        public async Task MarkEventAsProcessedAsync(string eventId)
        {
            var appliedEvent = new AppliedEvent
            {
                EventId = eventId,
                ProjectorName = ProjectorName,
                AppliedAt = DateTime.UtcNow,
                Position = await GetLastProcessedPositionAsync()
            };

            _dbContext.AppliedEvents.Add(appliedEvent);
            await _dbContext.SaveChangesAsync();
        }

        protected abstract Task ProcessEventInternalAsync(IEvent @event, long position);
    }
}
