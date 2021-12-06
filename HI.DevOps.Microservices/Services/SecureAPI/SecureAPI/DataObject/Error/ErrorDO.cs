using Hi.DevOps.TimeSheet.API.Common.Enum;

namespace Hi.DevOps.TimeSheet.API.DataObject.Error
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