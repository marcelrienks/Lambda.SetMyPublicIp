using Amazon.Route53;
using System.Threading.Tasks;

namespace Lambda.SetMyPublicIp.Tests
{
    public class MockRouteHandler : IRouteHandler
    {
        public Task<ChangeStatus> UpsertRecordset(string hostedZoneId, string domain, string ipAddress)
        {
            return Task.FromResult(new ChangeStatus("OK"));
        }
    }
}
