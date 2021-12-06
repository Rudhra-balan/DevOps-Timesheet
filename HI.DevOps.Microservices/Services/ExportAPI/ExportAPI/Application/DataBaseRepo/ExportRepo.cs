using System;
using System.Collections.Generic;
using System.Reflection;
using HI.DevOps.DatabaseContext.ConnectionManager;
using HI.DevOps.DatabaseContext.ConnectionManager.SafeDataReader;
using Hi.DevOps.Export.API.Application.Constants;
using Hi.DevOps.Export.API.Application.IDataBaseRepo;
using Hi.DevOps.Export.API.Common;
using Hi.DevOps.Export.API.Common.Enum;
using Hi.DevOps.Export.API.DataObject.ExportDO;
using log4net;
using ApplicationException = Hi.DevOps.Export.API.Common.Exception.ApplicationException;

namespace Hi.DevOps.Export.API.Application.DataBaseRepo
{
    public class ExportRepo : IExportRepo
    {
        #region constructor

        public ExportRepo(string connectionString)
        {
            _connectionString = connectionString;
            SysLog = LogManager.GetLogger(typeof(ExportRepo));
        }

        #endregion

        #region Private

        private readonly string _connectionString;
        public ILog SysLog { get; }

        #endregion

        #region Public Member

      
        public List<ExportDO> ExportTimeSheet(string queryString)
        {
            var timeSheetList = new List<ExportDO>();
            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogEnteringMethodInfo,
                    $"{typeof(ExportRepo)}_{MethodBase.GetCurrentMethod()}"));
            try
            {
                var query = string.Format(RepoConstants.SQL_EXPORT_TIMESHEET, queryString);
                using var command = SqlConnectionManager.GetDbCommand(_connectionString, query);
                command.Connection.Open();
                using var dataReader = new SafeDataReader(command.ExecuteReader());
                while (dataReader.Read())
                    timeSheetList.Add(new ExportDO
                    {
                        Username = dataReader.GetString("Username"),
                        Email = dataReader.GetString("EmailAddress"),
                        Department = dataReader.GetInt32("DepartmentId"),
                        Epic = dataReader.GetString("Epic"),
                        Feature = dataReader.GetString("Feature"),
                        UserStory = dataReader.GetString("UserStory"),
                        Requirement = dataReader.GetString("Requirements"),
                        Project = dataReader.GetString("Project"),
                        Task = dataReader.GetString("Task"),
                        Week1 = dataReader.GetInt32("Week1"),
                        Week2 = dataReader.GetInt32("Week2"),
                        Week3 = dataReader.GetInt32("Week3"),
                        Week4 = dataReader.GetInt32("Week4"),
                        Week5 = dataReader.GetInt32("Week5"),
                        Total = dataReader.GetInt32("Total")
                    });
                command.Connection.Close();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"{ErrorEnum.DataExportTimeSheetByDateAndDepartError.GetDescription()} Error Message {ex.Message}",
                    ErrorEnum.DataExportTimeSheetByDateAndDepartError, ex);
            }

            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogExitingMethodInfo,
                    $"{typeof(ExportRepo)}_{MethodBase.GetCurrentMethod()}"));

            return timeSheetList;
        }

        public List<string> GetAllUser()
        {
            var allUser = new List<string>();
            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogEnteringMethodInfo,
                    $"{typeof(ExportRepo)}_{MethodBase.GetCurrentMethod()}"));
            try
            {
                var queryString = RepoConstants.SQL_GET_ALL_USER;
                using var command = SqlConnectionManager.GetDbCommand(_connectionString, queryString);
                command.Connection.Open();
                using var dataReader = new SafeDataReader(command.ExecuteReader());
                while (dataReader.Read())
                    allUser.Add(dataReader.GetString("EmailAddress"));
                command.Connection.Close();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"{ErrorEnum.DataExportTimeSheetByDateAndDepartError.GetDescription()} Error Message {ex.Message}",
                    ErrorEnum.DataExportTimeSheetByDateAndDepartError, ex);
            }

            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogExitingMethodInfo,
                    $"{typeof(ExportRepo)}_{MethodBase.GetCurrentMethod()}"));

            return allUser;
        }

        #endregion
    }
}