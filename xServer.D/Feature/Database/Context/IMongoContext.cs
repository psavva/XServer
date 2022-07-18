using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace x42.Feature.Database.Context
{
    public interface IMongoContext
    {
        MongoClient _mongoClient { get; set; }
        IClientSessionHandle _session { get; set; }
        void AddCommand(Func<Task> func);
        void Dispose();
        IMongoCollection<T> GetCollection<T>(string name);
        Task<int> SaveChanges();
    }
}