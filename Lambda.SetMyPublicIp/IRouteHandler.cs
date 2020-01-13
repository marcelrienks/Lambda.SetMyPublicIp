using Amazon.Route53;
using System.Threading.Tasks;

namespace Lambda.SetMyPublicIp
{
    public interface IRouteHandler
    {
        Task<ChangeStatus> UpsertRecordset(string hostedZoneId, string domain, string ipAddress);
    }
}