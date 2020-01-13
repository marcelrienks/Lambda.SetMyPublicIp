using Amazon.Route53;
using Amazon.Route53.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lambda.SetMyPublicIp
{
    public class RouteHandler : IRouteHandler
    {
        private IAmazonRoute53 _route53Client;

        public RouteHandler(IAmazonRoute53 route53Client)
        {
            _route53Client = route53Client;
        }

        /// <summary>
        /// Creates if none exists, else updates an existing record set of type A, assigning the supplied public ip address to the supplied domain
        /// </summary>
        /// <param name="hostedZoneId">teh hosted zone id to create or update a recordset for</param>
        /// <param name="domain">the domain to assign the public ip to</param>
        /// <param name="ipAddress">teh public ip to assign to the supplied domain</param>
        /// <returns></returns>
        public async Task<ChangeStatus> UpsertRecordset(string hostedZoneId, string domain, string ipAddress)
        {
            var response = await _route53Client.ChangeResourceRecordSetsAsync(new ChangeResourceRecordSetsRequest
            {
                ChangeBatch = new ChangeBatch
                {
                    Changes = new List<Change> {
                        new Change {
                            Action = "UPSERT",
                            ResourceRecordSet = new ResourceRecordSet {
                                Name = domain,
                                ResourceRecords = new List<ResourceRecord> { new ResourceRecord { Value = ipAddress } },
                                TTL = 60,
                                Type = "A"
                            }
                        }
                    },
                    Comment = "Updating the recordset to match the new public IP address"
                },
                HostedZoneId = hostedZoneId
            });

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                return response.ChangeInfo.Status;

            else
                throw new Exception();
        }
    }
}
