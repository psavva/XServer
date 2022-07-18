using Microsoft.EntityFrameworkCore;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using x42.Controllers.Requests;
using x42.Feature.Database;
using x42.Feature.Database.Context;
using x42.Feature.Database.Tables;
using x42.Feature.Network;
using x42.Feature.PowerDns;
using x42.Feature.Profile;
using x42.Feature.WordPressPreview.Models;

namespace x42.Feature.WordPressPreview
{
    public partial class WordPressManager
    {

        private readonly PowerDnsFeature _powerDnsFeature;
        private readonly NetworkFeatures _networkFeatures;
        private readonly DatabaseSettings _databaseSettings;

        public WordPressManager(PowerDnsFeature powerDnsFeature, NetworkFeatures networkFeatures, DatabaseSettings databaseSettings)
        {
            _powerDnsFeature = powerDnsFeature;
            _networkFeatures = networkFeatures;
            _databaseSettings = databaseSettings;
        }

        public async Task<List<string>> GetWordPressPreviewDomains()
        {
            try
            {

                var result = new List<string>();

                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(10);

                    result.Add(RandName());
                }
                return await Task.FromResult(result);

            }
            catch (Exception ex)
            {

                return null;
            }
        }



        public async Task<WordpressDomainResult> ReserveWordpressPreviewDNS(WordPressReserveRequest registerRequest)
        {
            WordpressDomainResult result = new WordpressDomainResult();


            using (X42DbContext dbContext = new X42DbContext(_databaseSettings.ConnectionString))
            {
                var hasEntry = dbContext.Domains.Where(l => l.Name == registerRequest.Name).Count() > 0;

                if (hasEntry)
                {

                    result.Success = false;
                    result.ResultMessage = "Domain Already Registered.";
                    return result;

                }
            }


            bool isRegisterKeyValid = await _networkFeatures.IsRegisterKeyValid(registerRequest.Name, registerRequest.KeyAddress, registerRequest.ReturnAddress, registerRequest.Signature);
            if (!isRegisterKeyValid)
            {
                result.Success = false;
                result.ResultMessage = "Signature validation failed.";
                return result;
            }

            // Price Lock ID does not exist, this is a new request, so let's create a price lock ID for it, and reserve the name.
            var profilePriceLockRequest = new CreatePriceLockRequest()
            {
                DestinationAddress = _networkFeatures.GetMyFeeAddress(),
                RequestAmount = 0.5m,      // $0.50
                RequestAmountPair = 1,  // USD
                ExpireBlock = 15
            };
            var newPriceLock = await _networkFeatures.CreateNewPriceLock(profilePriceLockRequest);
            if (newPriceLock == null || string.IsNullOrEmpty(newPriceLock?.PriceLockId) || newPriceLock?.ExpireBlock <= 0)
            {
                result.Success = false;
                result.ResultMessage = "Failed to acquire a price lock";
                return result;
            }

            int status = (int)Status.Reserved;
            var newProfile = new DomainData()
            {
                Name = registerRequest.Name,
                KeyAddress = registerRequest.KeyAddress,
                ReturnAddress = registerRequest.ReturnAddress,
                PriceLockId = newPriceLock.PriceLockId,
                Signature = registerRequest.Signature,
                Status = status,
                Relayed = false
            };
            using (X42DbContext dbContext = new X42DbContext(_databaseSettings.ConnectionString))
            {
                var newRecord = dbContext.Domains.Add(newProfile);
                if (newRecord.State == EntityState.Added)
                {
                    try
                    {
                        dbContext.SaveChanges();

                    }
                    catch (Exception e)
                    {

                        throw;
                    }
                    result.PriceLockId = newPriceLock.PriceLockId;
                    result.Status = status;
                    result.Success = true;
                }
                else
                {
                    result.Status = (int)Status.Rejected;
                    result.ResultMessage = "Failed to add domain.";
                    result.Success = false;
                }
            }

            try
            {
                await _powerDnsFeature.AddNewSubDomain(registerRequest.Name);

            }
            catch (Exception e)
            {

                result.Success = false;
                result.ResultMessage = "Failed to register subdomain";
                return result;
            }



            return result;
        }

        public async Task ProvisionWordPress(string subdomain)
        {

            try
            {
                using (var client = new SshClient("185.197.194.25", "root", "80Qb562c0e"))
                {

                    var email = "dimsavva@gmail.com";

                    client.Connect();

                    var commndList = new List<SshCommandModel>();

                    commndList.Add(new SshCommandModel() { Command = "cd x42-Server-Deployment/wordpress", Description = "changing direc", Timeout = 1000 });

                    commndList.Add(new SshCommandModel() { Command = "./deploy_site.sh wordpress " + subdomain + " " + email + " c89e78b0ee534b14bd7ae093388bfeb0 c89e78b0ee534b14bd7ae093388bfeb0", Description = "provisioning", Timeout = 38000 });

                    InteractiveShellStream shell = new InteractiveShellStream(client.CreateShellStream("xterm-mono", 80, 24, 800, 600, 1000));
                    Console.Write(shell.ReadWithTimeout(1000));



                    if (client.IsConnected)
                    {

                        foreach (var command in commndList)
                        {

                            shell.WriteLine(command.Command);
                            shell.ReadWithTimeout(command.Timeout);


                        }

                    }


                }

            }
            catch (Exception e)
            {

                throw;
            }

            await Task.CompletedTask;

        }

        public List<string> GetWordpressRegisteredPreviewDomains() {

            var domains = new List<string>();

            using (X42DbContext dbContext = new X42DbContext(_databaseSettings.ConnectionString))
            {
                domains = dbContext.Domains.Select(l => l.Name).ToList();
            }

            return domains;
            }

        private string RandName()
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            string[] color = new string[] { "blue", "red", "yellow", "orange", "green", "pink", "black", "white", "violet", "magenta", "brown", "golden" };
            string[] emotion = new string[] { "happy", "sad", "excited", "angry", "nervous", "amused", "anxious", "awkward", "calm", "confused", "joyful", "romantic", "surprised" };
            string[] animal = new string[] { "fox", "rabbit", "cow", "dog", "cat", "horse", "tiger", "lion", "shark", "wolf", "bear", "koala", "panda", "giraffe", "turtle", "deer", "cheetah" };

            string domain = "x42.site";
            using (X42DbContext dbContext = new X42DbContext(_databaseSettings.ConnectionString))
            {
                var x42sitecount = dbContext.Domains.Where(l => l.Name.Contains("x42.site")).Count();
                var x42dashsitecount = dbContext.Domains.Where(l => l.Name.Contains("x-42.site")).Count();
                var x42onlinecount = dbContext.Domains.Where(l => l.Name.Contains("x42.online")).Count();
                var x42cloudcount = dbContext.Domains.Where(l => l.Name.Contains("x42.cloud")).Count();
                var x42appcount = dbContext.Domains.Where(l => l.Name.Contains("x42.app")).Count();


                if (x42dashsitecount < x42sitecount)
                {
                    domain = "x-42.site";
                }

                if (x42onlinecount < x42dashsitecount)
                {
                    domain = "x42.online";
                }
                if (x42cloudcount < x42onlinecount)
                {
                    domain = "x42.cloud";
                }

                if (x42appcount < x42cloudcount)
                {
                    domain = "x42.app";
                }
            }

            var name = emotion[rand.Next(0, emotion.Length)] + "-" + color[rand.Next(0, color.Length)] + "-" + animal[rand.Next(0, animal.Length)] + "." + domain;


            return name;

        }
    }
}
