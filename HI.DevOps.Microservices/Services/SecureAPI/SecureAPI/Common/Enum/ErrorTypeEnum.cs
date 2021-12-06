namespace Hi.DevOps.TimeSheet.API.Common.Enum
{
    public enum ErrorTypeEnum
    {
        // Error display is information -- e.g. 32 rows deleted, successfully renamed etc.,
        Information = 1,

        // Error type is catastropic
        Error = 2,

        // error type is validation
        Validation = 3,

        // Error Type is warning 
        Warning = 4
    }
}