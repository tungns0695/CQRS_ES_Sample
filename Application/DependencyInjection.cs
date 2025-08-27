using Application.CommandHandlers.Taxpayer;
using Application.Domains;
using Application.Domains.EventHandlers;
using Application.QueryHandlers;
using Application.Queries;
using Application.ReadModels;
using Infrastructure.Aggregate;
using Infrastructure.Events;
using Infrastructure.Queries;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Infrastructure.BackGroundService;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            BsonClassMap.RegisterClassMap<AggregateRoot>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.AddKnownType(typeof(TaxpayerAggregate));
            });
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            services.AddMassTransit(x =>
            {
                // Register command handlers as consumers
                x.AddConsumer<CreateTaxpayerCommandHandler>();
                x.AddConsumer<UpdateTaxpayerCommandHandler>();
                x.AddConsumer<DeactivateTaxpayerCommandHandler>();
                x.AddConsumer<AddTaxpayerAddressCommandHandler>();
                x.AddConsumer<UpdateTaxpayerAddressCommandHandler>();
                x.AddConsumer<RemoveTaxpayerAddressCommandHandler>();
                x.AddConsumer<UpdateTaxpayerEmploymentCommandHandler>();
                x.AddConsumer<UpdateTaxpayerTaxFilingStatusCommandHandler>();

                // Register query handlers as consumers
                x.AddConsumer<GetTaxpayersQueryHandler>();
                x.AddConsumer<GetTaxpayerQueryHandler>();
                x.AddConsumer<GetTaxpayersByFilingStatusQueryHandler>();
                x.AddConsumer<GetTaxpayersByEmploymentStatusQueryHandler>();
                x.AddConsumer<GetTaxpayersWithOutstandingBalanceQueryHandler>();
                x.AddConsumer<GetTaxpayersEligibleForRefundQueryHandler>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h => {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddMediator(x =>
            {
                // Register event handlers as consumers
                x.AddConsumer<TaxpayerCreatedEventHandler>();
                x.AddConsumer<TaxpayerUpdatedEventHandler>();
                x.AddConsumer<TaxpayerDeactivatedEventHandler>();
                x.AddConsumer<TaxpayerAddressAddedEventHandler>();
                x.AddConsumer<TaxpayerAddressUpdatedEventHandler>();
                x.AddConsumer<TaxpayerAddressRemovedEventHandler>();
                x.AddConsumer<TaxpayerEmploymentUpdatedEventHandler>();
                x.AddConsumer<TaxpayerTaxFilingStatusUpdatedEventHandler>();
            });

            services.AddDbContext<TaxSystemDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

            services.AddSingleton<IMongoClient>(provider =>
            {
                var connectionString = configuration.GetConnectionString("MongoDB");
                return new MongoClient(connectionString);
            });

            services.AddScoped(provider =>
            {
                var client = provider.GetRequiredService<IMongoClient>();
                var databaseName = configuration.GetSection("MongoDBSettings:DatabaseName").Value ?? "TaxSystem";
                return client.GetDatabase(databaseName);
            });

            // Register event sourcing services
            services.AddScoped<IEventStoreReader, EventStoreReader>();
            services.AddScoped<IProjectorDbContext, ProjectorDbContextAdapter>();
            services.AddScoped<IEventProjector, TaxpayerEventPublisher>();
            services.AddHostedService<ProjectorBackgroundService>();

            return services;
        }
    }
}