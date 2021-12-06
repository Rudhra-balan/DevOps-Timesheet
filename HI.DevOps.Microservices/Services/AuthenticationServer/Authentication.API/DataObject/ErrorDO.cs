using Hi.DevOps.Authentication.API.Common.Enum;

namespace Hi.DevOps.Authentication.API.DataObject
{
    public class ErrorDo
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public ErrorTypeEnum Type { get; set; }

        public string ResponseValue { get; set; }
    }

    public static class ErrorDoExtensions
    {
        public static bool HasError(this ErrorDo errorDo)
        {
            if (errorDo == null) return false;
            return errorDo.Id > 0;
        }
    }
}