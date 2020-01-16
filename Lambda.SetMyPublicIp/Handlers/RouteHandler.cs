using Amazon.Route53;
using Amazon.Route53.Model;
using Lambda.SetMyPublicIp.Interfaces;
using System.Collections.Generic;
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
        /// <param name="hostedZoneId">the hosted zone id to create or update a recordset for</param>
        /// <param name="domainName">the domain to assign the public ip to</param>
        /// <param name="publicIp">the public ip to assign to the supplied domain</param>
        /// <returns>the change request id</returns>
        public async Task<string> UpsertRecordset(string hostedZoneId, string domainName, string publicIp)
        {
            var recordSet = new ResourceRecordSet
            {
                Name = domainName,
                TTL = 60,
                Type = RRType.A,
                ResourceRecords = new List<ResourceRecord> { new ResourceRecord { Value = publicIp } }
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

            return changeRequest.Id;
        }
    }
}
