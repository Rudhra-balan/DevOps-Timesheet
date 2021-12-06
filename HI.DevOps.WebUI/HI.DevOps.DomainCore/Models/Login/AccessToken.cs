namespace HI.DevOps.DomainCore.Models.Login
{
    public sealed class AccessToken
    {
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
    }
}