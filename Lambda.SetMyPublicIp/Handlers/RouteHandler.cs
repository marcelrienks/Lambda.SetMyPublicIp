using Amazon.Route53;
using Amazon.Route53.Model;
using Lambda.SetMyPublicIp.Helpers;
using Lambda.SetMyPublicIp.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lambda.SetMyPublicIp.Handlers
{
    public class RouteHandler : IRouteHandler
    {
        private IAmazonRoute53 _route53Client;

        public RouteHandler(IAmazonRoute53 route53Client)
        {
            _route53Client = route53Client;
        }

        /// <summary>
        /// Creates a record set if none exists, else updates an existing record set of type A, assigning the supplied public ip address to the supplied domain
        /// </summary>
        /// <param name="hostedZoneId">teh hosted zone id to create or update a recordset for</param>
        /// <param name="domain">the domain to assign the public ip to</param>
        /// <param name="ipAddress">teh public ip to assign to the supplied domain</param>
        /// <returns></returns>
        public async Task<ChangeStatus> UpsertRecordset(string hostedZoneId, string domain, string ipAddress)
        {
            var recordSet = new ResourceRecordSet
            {
                Name = domain,
                TTL = 60,
                Type = RRType.A,
                ResourceRecords = new List<ResourceRecord> { new ResourceRecord { Value = ipAddress } }
            };

            var change = new Change
            {
                ResourceRecordSet = recordSet,
                Action = ChangeAction.UPSERT
            };

            var changeBatch = new ChangeBatch { Changes = new List<Change> { change } };

            var recordsetRequest = new ChangeResourceRecordSetsRequest { HostedZoneId = hostedZoneId, ChangeBatch = changeBatch };

            var recordsetResponse = await _route53Client.ChangeResourceRecordSetsAsync(recordsetRequest);

            var changeRequest = new GetChangeRequest { Id = recordsetResponse.ChangeInfo.Id };

            // Wait for the status to change from Pending to Insync
            while ((await _route53Client.GetChangeAsync(changeRequest)).ChangeInfo.Status == ChangeStatus.PENDING)
            {
                Logging.Log("Change is pending...");
                await Task.Delay(TimeSpan.FromSeconds(15));
            }

            return ChangeStatus.INSYNC;
        }
    }
}
