namespace Hi.DevOps.Export.API.Common.Exception
{
    public class ForbiddenException : ApplicationException
    {
        public ForbiddenException(string message) : base(message)
        {
        }
    }
}