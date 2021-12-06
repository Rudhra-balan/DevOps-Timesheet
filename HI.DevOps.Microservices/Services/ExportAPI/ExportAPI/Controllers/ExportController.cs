using System.Collections.Generic;
using Hi.DevOps.Export.API.Application.IBusinessManager;
using Hi.DevOps.Export.API.Common;
using Hi.DevOps.Export.API.DataObject.ExportDO;
using Microsoft.AspNetCore.Mvc;

namespace Hi.DevOps.Export.API.Controllers
{
    public class ExportController : BaseController
    {
        #region Private Variable

        private readonly IExportBM _iExportBM;

        #endregion

        #region Constructor

        public ExportController(IExportBM iExportBM)
        {
            _iExportBM = iExportBM;
        }

        #endregion

        #region Public Member

        [HttpPost("ExportTimeSheet")]
        public List<ExportDO> ExportTimeSheet([FromBody] string requestQuery)
        {
            if(requestQuery.IsNullOrEmpty()) return  new List<ExportDO>();
            return _iExportBM.ExportTimeSheet(requestQuery);
        }

        [HttpGet("GetAllUser")]
        public List<string> ExportTimeSheet()
        {
            return _iExportBM.GetAllUser();
        }
        #endregion
    }
}