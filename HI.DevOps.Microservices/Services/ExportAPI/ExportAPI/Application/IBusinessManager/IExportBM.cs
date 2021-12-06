using System.Collections.Generic;
using Hi.DevOps.Export.API.DataObject.ExportDO;

namespace Hi.DevOps.Export.API.Application.IBusinessManager
{
    public interface IExportBM
    {
        List<ExportDO> ExportTimeSheet(string requestQuery);

        List<string> GetAllUser();
    }
}