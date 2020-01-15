namespace Lambda.SetMyPublicIp.Models
{
    public class Request
    {
        public string HostedZoneId { get; set; }
        public string DomainName { get; set; }
        public string PublicIps { get; set; }
    }
}
