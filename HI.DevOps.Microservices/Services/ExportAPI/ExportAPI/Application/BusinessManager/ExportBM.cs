using System.Collections.Generic;
using System.Linq;
using Hi.DevOps.Export.API.Application.IBusinessManager;
using Hi.DevOps.Export.API.Application.IDataBaseRepo;
using Hi.DevOps.Export.API.DataObject.ExportDO;

namespace Hi.DevOps.Export.API.Application.BusinessManager
{
    public class ExportBM : IExportBM
    {
        #region constructor

        public ExportBM(IExportRepo exportRepo)
        {
            ExportRepo = exportRepo;
        }

        #endregion

        #region Private variable

        private IExportRepo ExportRepo { get; }

        #endregion

        #region Public Member

        public List<ExportDO> ExportTimeSheet(string requestQuery)
        {
            var timeSheetList = new List<ExportDO>();
            try
            {
                timeSheetList = ExportRepo.ExportTimeSheet(requestQuery);
            }
            catch
            {
                //
            }

            return timeSheetList;
        }

        public List<string> GetAllUser()
        {
            var userList = new List<string>();
            try
            {
                userList = ExportRepo.GetAllUser();
            }
            catch
            {
                //
            }

            return userList;
        }

        #endregion
    }
}