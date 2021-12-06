namespace Hi.DevOps.Export.API.Common.Exception
{
    public class UnauthorizedException : ApplicationException
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}