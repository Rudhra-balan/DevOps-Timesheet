namespace Hi.DevOps.Authentication.API.Common.Exception
{
    public class ConflictException : ApplicationException
    {
        public ConflictException(string message) : base(message)
        {
        }

        public ConflictException(string message, System.Exception ex) : base(message, ex)
        {
        }
    }
}