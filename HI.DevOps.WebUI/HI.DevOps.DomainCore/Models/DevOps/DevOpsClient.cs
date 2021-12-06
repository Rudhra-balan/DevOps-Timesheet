namespace HI.DevOps.DomainCore.Models.DevOps
{
    public class DevOpsClient
    {
        public string AuthUrl { get; set; }
        public string TokenUrl { get; set; }
        public string ProfileUrl { get; set; }
        public string CallBackUrl { get; set; }
        public string ClientAppId { get; set; }
        public string ClientAppSecret { get; set; }
        public string ClientSecret { get; set; }
        public string Scopes { get; set; }
    }
}