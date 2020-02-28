﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X42.Controllers.Requests;
using X42.Feature.Database.Context;
using X42.Feature.Database.Tables;
using X42.Server.Results;

namespace x42.Server
{
    public class SetupServer
    {
        private string ConnectionString { get; set; }

        public enum Status
        {
            NotStarted = 1,
            Started = 2,
            Complete = 3
        }

        public SetupServer(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public bool AddServerToSetup(SetupRequest setupRequest)
        {
            bool result = false;

            using (X42DbContext dbContext = new X42DbContext(ConnectionString))
            {
                IQueryable<ServerData> serverNodes = dbContext.Servers;
                if (serverNodes.Count() == 0)
                {
                    ServerData serverData = new ServerData()
                    {
                        PublicAddress = setupRequest.Address,
                        DateAdded = DateTime.UtcNow
                    };

                    var newRecord = dbContext.Add(serverData);
                    if (newRecord.State == EntityState.Added)
                    {
                        dbContext.SaveChanges();
                        result = true;
                    }
                }
            }
            return result;
        }

        public SetupStatusResult GetServerSetupStatus()
        {
            SetupStatusResult result = new SetupStatusResult() { ServerStatus = Status.NotStarted };

            using (X42DbContext dbContext = new X42DbContext(ConnectionString))
            {
                IQueryable<ServerData> server = dbContext.Servers;
                if (server.Count() > 0)
                {
                    result.ServerStatus = Status.Started;

                    string publicAddress = server.First().PublicAddress;

                    IQueryable<ServerNodeData> serverNode = dbContext.ServerNodes.Where(s => s.PublicAddress == publicAddress);
                    if (serverNode.Count() > 0)
                    {
                        result.ServerStatus = Status.Complete;
                    }
                }
            }

            return result;
        }
    }
}
