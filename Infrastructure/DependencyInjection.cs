using Infrastructure.Aggregate;
using Infrastructure.EventStore;
using Infrastructure.Repository;
using Infrastructure.Snapshotting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped(typeof(IAggregateRepository<>), typeof(AggregateRepository<>));
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<ISnapshotRepository, SnapshotRepository>();
            services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
            services.AddScoped(typeof(IReadRepository<,>), typeof(ReadRepository<,>));
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            return services;
        }
    }
}