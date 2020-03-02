﻿using System.Linq;
using X42.Controllers.Models;
using X42.Feature.Database.Context;
using X42.Feature.Database.Tables;

namespace x42.Server
{
    public class ServerFunctions
    {
        private string ConnectionString { get; set; }

        public ServerFunctions(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public TopResult GetTopXServers(int top)
        {
            TopResult result = new TopResult();

            using (X42DbContext dbContext = new X42DbContext(ConnectionString))
            {
                IQueryable<ServerNodeData> servers = dbContext.ServerNodes.OrderByDescending(u => u.Priority).Take(top);
                if (servers.Count() > 0)
                {
                    servers.ToList().ForEach(
                        x => result.XServers.Add(
                            new XServerConnectionInfo()
                            {
                                Name = x.Name,
                                Address = x.NetworkAddress,
                                Port = x.NetworkPort,
                                Priotiry = x.Priority
                            }
                    ));
                }
            }

            return result;
        }
    }
}