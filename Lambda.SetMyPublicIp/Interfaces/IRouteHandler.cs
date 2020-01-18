using System.Threading.Tasks;

namespace Lambda.SetMyPublicIp.Interfaces
{
    public interface IRouteHandler
    {
        Task<(string, string)> UpsertRecordset(string hostedZoneId, string domainName, string publicIp);
    }
}