﻿using System;
using System.Collections.Generic;
using NBitcoin;
using NBitcoin.Protocol;
using NodeSeedData = X42.Utilities.NodeSeedData;

namespace X42.ServerNode
{
    public abstract class ServerNodeBase
    {
        /// <summary>
        ///     The default port on which servers of this servernode communicate with external clients.
        /// </summary>
        public int DefaultPort { get; protected set; }

        /// <summary>
        ///     The name of the servernode.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        ///     The default name used for the servernode configuration file.
        /// </summary>
        public string DefaultConfigFilename { get; protected set; }

        /// <summary>
        ///     The list of tiers available for the servernode.
        /// </summary>
        public List<Tier> Tiers { get; protected set; }

        /// <summary>
        ///     The list of servers on the servernode that our current server tries to connect to.
        /// </summary>
        public List<NetworkAddress> SeedServers { get; protected set; }

        /// <summary>
        ///     The Grade period (In Minutes) when for how long a node is offline before inactive.
        /// </summary>
        public long GracePeriod { get; protected set; }

        /// <summary>
        ///     The list of DNS seeds from which to get IP addresses when bootstrapping a server.
        /// </summary>
        public List<NodeSeedData> NodeSeeds { get; protected set; }

        protected IEnumerable<NetworkAddress> ConvertToNetworkAddresses(string[] seeds, int defaultPort)
        {
            Random rand = new Random();
            TimeSpan oneWeek = TimeSpan.FromDays(7);

            foreach (string seed in seeds)
                // It'll only connect to one or two seed servers because once it connects,
                // it'll get a pile of addresses with newer timestamps.
                // Seed servers are given a random 'last seen time' of between one and two weeks ago.
                yield return new NetworkAddress
                {
                    Time = DateTime.UtcNow - TimeSpan.FromSeconds(rand.NextDouble() * oneWeek.TotalSeconds) - oneWeek,
                    Endpoint = Utils.ParseIpEndpoint(seed, defaultPort)
                };
        }
    }
}