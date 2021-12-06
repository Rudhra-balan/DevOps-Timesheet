using System.Collections.Generic;

namespace Hi.DevOps.Export.API.DataObject
{
    public class BaseDo
    {
        public BaseDo()
        {
            ErrorListDo = new List<ErrorDO.ErrorDO>();
        }

        public List<ErrorDO.ErrorDO> ErrorListDo { get; set; }
    }
}