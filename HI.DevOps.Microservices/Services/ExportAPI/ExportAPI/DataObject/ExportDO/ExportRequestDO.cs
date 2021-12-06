using System.Collections.Generic;

namespace Hi.DevOps.Export.API.DataObject.ExportDO
{
    public class ExportRequestDO
    {
        public ExportRequestDO()
        {
            DepartmentIds = new List<int>();
        }

        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public List<int> DepartmentIds { get; set; }
    }
}