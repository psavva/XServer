using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using x42.Feature.Database.Context;
using x42.Configuration;
using x42.Configuration.Logging;
using x42.Feature.Setup;
using x42.ServerNode;
using x42.Server;
using x42.Feature.Database.UoW;
using x42.Feature.Database.Repositories;
using x42.Feature.Database.Tables;
using x42.Feature.Database.Repositories.Profiles;
using MongoDB.Driver;

namespace x42.Feature.Database
{
    /// <inheritdoc />
    /// <summary>
    ///     Provides an ability to communicate with different database types.
    /// </summary>
    public class DatabaseFeatures : ServerFeature
    {
        /// <summary>Instance logger.</summary>
        private readonly ILogger _logger;

        /// <summary>Instance logger.</summary>
        private readonly DatabaseSettings _databaseSettings;

        public IDataStore dataStore { get; set; }

        public bool DatabaseConnected { get; set; } = false;

        private readonly IProfileReservationRepository _profileReservationRepository;


        public DatabaseFeatures(
            ServerNodeBase network,
            ILoggerFactory loggerFactory,
            DatabaseSettings databaseSettings
,
            IProfileReservationRepository profileReservationRepository
            )
        {
            _logger = loggerFactory.CreateLogger(GetType().FullName);
            _databaseSettings = databaseSettings;
            _profileReservationRepository = profileReservationRepository;
            dataStore = new DataStore(loggerFactory, databaseSettings, _profileReservationRepository);

        }

        /// <summary>
        ///     Prints command-line help.
        /// </summary>
        /// <param name="serverNodeBase">The servernode to extract values from.</param>
        public static void PrintHelp(ServerNodeBase serverNodeBase)
        {
            DatabaseSettings.PrintHelp(serverNodeBase);
        }

        /// <summary>
        ///     Get the default configuration.
        /// </summary>
        /// <param name="builder">The string builder to add the settings to.</param>
        /// <param name="network">The network to base the defaults off.</param>
        public static void BuildDefaultConfigurationFile(StringBuilder builder, ServerNodeBase network)
        {
            DatabaseSettings.BuildDefaultConfigurationFile(builder, network);
        }

        /// <summary>
        ///     Connect to the database.
        /// </summary>
        public void Connect()
        {
            _logger.LogInformation("Connected to database");
        }

        public void Disconnect()
        {
            _logger.LogInformation("Disconnected from database");
        }

        /// <inheritdoc />
        public override Task InitializeAsync()
        {
            try
            {
                using (X42DbContext dbContext = new X42DbContext(_databaseSettings.ConnectionString))
                {
                    _logger.LogInformation("Connecting to database");

                    dbContext.Database.Migrate();

                    DatabaseConnected = true;

                    _logger.LogInformation("Database Feature Initialized");
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Database failed to Initialize.", ex);
                _logger.LogTrace("(-)[INITIALIZE_EXCEPTION]");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            Disconnect();
        }

        /// <inheritdoc />
        public override void ValidateDependencies(IServerServiceProvider services)
        {
            if (string.IsNullOrEmpty(_databaseSettings.ConnectionString))
            {
                throw new ConfigurationException("Connection string is required.");
            }
        }
    }

    /// <summary>
    ///     A class providing extension methods for <see cref="DatabaseFeatures" />.
    /// </summary>
    public static class DatabaseBuilderExtension
    {
        /// <summary>
        ///     Adds SQL components to the server.
        /// </summary>
        /// <param name="serverBuilder">The object used to build the current node.</param>
        /// <returns>The server builder, enriched with the new component.</returns>
        public static IServerBuilder UseSql(this IServerBuilder serverBuilder)
        {
            LoggingConfiguration.RegisterFeatureNamespace<DatabaseFeatures>("database");

            serverBuilder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<DatabaseFeatures>()
                    .FeatureServices(services =>
                    {
                        services.AddSingleton<DatabaseFeatures>();
                        services.AddSingleton<DatabaseSettings>();
                        services.AddSingleton<IMongoContext, MongoContext>();
                        services.AddSingleton<IUnitOfWork, UnitOfWork>();
                        services.AddSingleton(typeof(IRepository<ProfileReservationData2>), typeof(MongoRepository<ProfileReservationData2>));
                        services.AddSingleton<IProfileReservationRepository, ProfileReservationRepository>();
                        services.AddSingleton<IProfileRepository, ProfileRepository>();

                    });
            });

            return serverBuilder;
        }

        public static IServerBuilder UseNoql(this IServerBuilder serverBuilder)
        {
            throw new NotImplementedException("NoSQL is not yet supported");
        }
    }
}