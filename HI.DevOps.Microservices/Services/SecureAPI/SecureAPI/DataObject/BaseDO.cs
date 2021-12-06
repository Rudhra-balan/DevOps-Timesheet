using System.Collections.Generic;
using Hi.DevOps.TimeSheet.API.DataObject.Error;

namespace Hi.DevOps.TimeSheet.API.DataObject
{
    public class BaseDo
    {
        public BaseDo()
        {
            ErrorListDo = new List<ErrorDO>();
        }

        public List<ErrorDO> ErrorListDo { get; set; }
    }
}