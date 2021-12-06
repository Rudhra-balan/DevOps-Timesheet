namespace Hi.DevOps.TimeSheet.API.Common.Exception
{
    public class BadRequestException : ApplicationException
    {
        public BadRequestException(string message) : base(message)
        {
        }

        public BadRequestException(string message, System.Exception ex) : base(message, ex)
        {
        }
    }
}