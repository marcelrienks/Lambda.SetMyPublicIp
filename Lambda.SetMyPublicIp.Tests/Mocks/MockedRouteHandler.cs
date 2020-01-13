using Amazon.Route53;
using Lambda.SetMyPublicIp.Interfaces;
using System.Threading.Tasks;

namespace Lambda.SetMyPublicIp.Tests.Mocks
{
    public class MockedRouteHandler : IRouteHandler
    {
        public Task<ChangeStatus> UpsertRecordset(string hostedZoneId, string domain, string ipAddress)
        {
            return Task.FromResult(new ChangeStatus("OK"));
        }
    }
}
