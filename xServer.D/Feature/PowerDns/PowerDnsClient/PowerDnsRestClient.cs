using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using x42.Feature.PowerDns.Models;
using RestSharp;

namespace x42.Feature.PowerDns.PowerDnsClient
{
    public partial class PowerDnsRestClient
    {

        /// <summary>Instance logger.</summary>
        private readonly ILogger _logger;
        private string _baseUrl;
        private string _apiKey;

        public PowerDnsRestClient(string baseUrl, string apiKey, ILogger mainLogger)
        {
            _logger = mainLogger;
            _baseUrl = baseUrl;
            _apiKey = apiKey;

        }

        /// <summary>
        ///     Gets All Zones
        /// </summary>
        public async Task<List<ZoneModel>> GetAllZones()
        {
            try
            {
                var client = new RestClient(_baseUrl);
                var request = new RestRequest("/api/v1/servers/localhost/zones", Method.Get);
                request.AddHeader("X-API-Key", _apiKey);
                var response = await client.ExecuteAsync<List<ZoneModel>>(request);

                return response.Data;
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"An Error Occured When looking up zones!", ex);

                return null;
            }
        }

        public async Task AddNewWordpressPreviewSubDomain(string subdomain)
        {

            var n1Host = "https://poweradmin.xserver.network";
            var n1Key = "cmp4V1Z0MnprRVRMbE10";

            var n3Host = "https://poweradmin2.xserver.network";
            var n3Key = "VnpKOXJ6eUdMcHV2S3I1";

            await AddNewSubDomain(subdomain, n1Host, n1Key);
            await AddNewSubDomain(subdomain, n3Host, n3Key);

        }

        public async Task AddNewSubDomain(string subdomain, string host, string apiKey)
        {
            var domain = "x42.site";

            if (subdomain.Contains("x-42.site"))
            {
                 domain = "x-42.site";
            }

            if (subdomain.Contains("x42.online"))
            {
                domain = "x42.online";
            }
            if (subdomain.Contains("x42.cloud"))
            {
                domain = "x42.cloud";
            }
            if (subdomain.Contains("x42.app"))
            {
                domain = "x42.app";
            }

            try
            {
                var client = new RestClient(host);
                var request = new RestRequest($"/api/v1/servers/localhost/zones/"+domain, Method.Patch);
                request.AddHeader("X-API-Key", apiKey);
                request.AddHeader("content-type", "application/json");

                var body = new DnsRequest() { Rrsets = new List<RRset>() { new RRset($"{subdomain}.", "REPLACE", 60, "A", "185.197.194.25") } };

                request.AddBody(body);
                var response = await client.ExecuteAsync(request);


            }
            catch (Exception ex)
            {
                _logger.LogDebug($"An Error Occured When add subdomain!", ex);

            }
        }

 

     
    }
}
