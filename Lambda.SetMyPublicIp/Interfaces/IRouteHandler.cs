using Amazon.Route53;
using System.Threading.Tasks;

namespace Lambda.SetMyPublicIp.Interfaces
{
    public interface IRouteHandler
    {
        Task<ChangeStatus> UpsertRecordset(string hostedZoneId, string domain, string ipAddress);
    }
}