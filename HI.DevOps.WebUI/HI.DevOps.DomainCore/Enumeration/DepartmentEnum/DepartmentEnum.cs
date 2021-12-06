using System.ComponentModel;

namespace HI.DevOps.DomainCore.Enumeration.DepartmentEnum
{
    public enum DepartmentEnum
    {
      
        #region API-Errors

        [Description("All")]
        All = 0,
        [Description("EEE-UI")]
        Ui = 1,
        [Description("EEE-FIRMWARE")] 
        Firmware = 2,
        [Description("MECHANICAL")] 
        Mechanical = 3

        #endregion
    }
}