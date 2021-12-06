namespace Hi.DevOps.TimeSheet.API.Common.Exception
{
    public class UnauthorizedException : ApplicationException
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}