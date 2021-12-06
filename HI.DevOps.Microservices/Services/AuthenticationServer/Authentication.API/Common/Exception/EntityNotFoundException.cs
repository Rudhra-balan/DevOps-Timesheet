namespace Hi.DevOps.Authentication.API.Common.Exception
{
    public class EntityNotFoundException : ApplicationException
    {
        public EntityNotFoundException(string message) : base(message)
        {
        }

        public EntityNotFoundException(string message, System.Exception ex) : base(message, ex)
        {
        }
    }
}