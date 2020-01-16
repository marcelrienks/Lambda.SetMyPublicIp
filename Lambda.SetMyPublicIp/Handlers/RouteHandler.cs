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
        /// <returns>the change request id and </returns>
        public async Task<(string, string)> UpsertRecordset(string hostedZoneId, string domainName, string publicIp)
        {
            // Create recordset change request
            var recordsetRequest = new ChangeResourceRecordSetsRequest(hostedZoneId, new ChangeBatch(new List<Change>()
            {
                new Change
                {
                    ResourceRecordSet = new ResourceRecordSet(domainName, RRType.A)
                    {
                        TTL = 60,
                        ResourceRecords = new List<ResourceRecord>() {
                            new ResourceRecord(publicIp)
                        }
                    },
                    Action = ChangeAction.UPSERT
                }
            }));

            // Reqeust change
            var recordsetResponse = await _route53Client.ChangeResourceRecordSetsAsync(recordsetRequest);
            if (recordsetResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                return (recordsetResponse.ChangeInfo.Id, recordsetResponse.ChangeInfo.Status);

            else
                throw new System.Exception($"Failed to Upsert recordset, ChangeResourceRecordSetsRequest status: {recordsetResponse.HttpStatusCode}");
        }
    }
}
