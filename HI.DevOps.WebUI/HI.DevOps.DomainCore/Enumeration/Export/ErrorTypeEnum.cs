using System.ComponentModel;

namespace HI.DevOps.DomainCore.Enumeration.Export
{
    public enum ExportDatabaseFieldEnum
    {
       [Description("Start-TimeSheetDate")]
       StartDate=1,
       [Description("End-TimeSheetDate")]
       EndDate = 2,
       [Description("userInfo.DepartmentId")]
       Department = 3,
       [Description("userInfo.EmailAddress")]
       User = 4
    }
}