namespace Hi.DevOps.Authentication.API.Common.Exception
{
    public class UnauthorizedException : ApplicationException
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}