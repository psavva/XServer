using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace x42.Feature.Database.Context
{
    public class MongoContext : IMongoContext
    {
        private IMongoDatabase _database { get; set; }
        public IClientSessionHandle _session { get; set; }
        public MongoClient _mongoClient { get; set; }
        private readonly List<Func<Task>> _commands;
        private readonly DatabaseSettings _databaseSettings;

        public MongoContext(DatabaseSettings databaseSettings)
        {

            // Every command will be stored and it'll be processed at SaveChanges
            _commands = new List<Func<Task>>();
            _databaseSettings = databaseSettings;
        }

        public async Task<int> SaveChanges()
        {
            ConfigureMongo();

            using (_session = await _mongoClient.StartSessionAsync())
            {
                var commandTasks = _commands.Select(c => c());

                await Task.WhenAll(commandTasks);
                _commands.Clear();

            }

            return _commands.Count;
        }

        private void ConfigureMongo()
        {
            if (_mongoClient != null)
            {
                return;
            }

            _mongoClient = new MongoClient(_databaseSettings.Mongoconnectionstring);
            _database = _mongoClient.GetDatabase(_databaseSettings.MongoDbName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            ConfigureMongo();

            return _database.GetCollection<T>(name);
        }

        public void Dispose()
        {
            _session?.Dispose();
            GC.SuppressFinalize(this);
        }

        public void AddCommand(Func<Task> func)
        {
            _commands.Add(func);
        }
    }
}
