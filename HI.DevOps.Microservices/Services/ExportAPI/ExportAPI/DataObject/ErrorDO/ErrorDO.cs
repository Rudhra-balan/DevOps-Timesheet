using Hi.DevOps.Export.API.Common.Enum;

namespace Hi.DevOps.Export.API.DataObject.ErrorDO
{
    public class ErrorDO
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public ErrorTypeEnum Type { get; set; }

        public string ResponseValue { get; set; }
    }

    public static class ErrorDoExtensions
    {
        public static bool HasError(this ErrorDO errorDo)
        {
            if (errorDo == null) return false;
            return errorDo.Id > 0;
        }
    }
}