using System.Collections.Generic;
using Hi.DevOps.Export.API.DataObject.ExportDO;

namespace Hi.DevOps.Export.API.Application.IDataBaseRepo
{
    public interface IExportRepo
    {
        List<ExportDO> ExportTimeSheet(string queryString);
        
        List<string> GetAllUser();
    }
}