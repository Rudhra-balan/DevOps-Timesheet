using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using HI.DevOps.DatabaseContext.ConnectionManager;
using HI.DevOps.DatabaseContext.ConnectionManager.SafeDataReader;
using Hi.DevOps.TimeSheet.API.Application.Constants;
using Hi.DevOps.TimeSheet.API.Application.IDataBaseRepo;
using Hi.DevOps.TimeSheet.API.Common;
using Hi.DevOps.TimeSheet.API.Common.Enum;
using Hi.DevOps.TimeSheet.API.DataObject.Error;
using Hi.DevOps.TimeSheet.API.DataObject.TimeSheet;
using log4net;
using ApplicationException = Hi.DevOps.TimeSheet.API.Common.Exception.ApplicationException;

namespace Hi.DevOps.TimeSheet.API.Application.DataBaseRepo
{
    public class TimeSheetRepo : ITimeSheetRepo
    {
        #region constructor

        public TimeSheetRepo(string connectionString)
        {
            _connectionString = connectionString;
            SysLog = LogManager.GetLogger(typeof(TimeSheetRepo));
        }

        #endregion

        #region Private

        private readonly string _connectionString;
        public ILog SysLog { get; }

        #endregion

        #region Public Member
        public ErrorDO SaveWeekTimeSheet(List<WeekTimeInfoDO> timeSheetList)
        {
            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogEnteringMethodInfo,
                    $"{typeof(TimeSheetRepo)}_{MethodBase.GetCurrentMethod()}"));
            if (!timeSheetList.Any())
                //Get the method name
                throw new ApplicationException(
                    $" {MethodBase.GetCurrentMethod()} is null in {typeof(TimeSheetRepo)}_{MethodBase.GetCurrentMethod()} Please check");
            var errorDO = new ErrorDO();
            try
            {
                var queryString = RepoConstants.SQL_INSERT_WEEKTIMESHEET;
                var parameters = new Dictionary<string, object>
                {
                    {"@UserId", SqlDbType.NVarChar},
                    {"@Project", SqlDbType.NVarChar},
                    {"@Epic", SqlDbType.NVarChar},
                    {"@Feature", SqlDbType.NVarChar},
                    {"@UserStory", SqlDbType.NVarChar},
                    {"@Requirements", SqlDbType.NVarChar},
                    {"@Task", SqlDbType.NVarChar},
                    {"@TimeSheetDate", SqlDbType.DateTime},
                    {"@TimeSheetHours", SqlDbType.Int}
                
                };
                using var command = SqlConnectionManager.GetDbCommand(_connectionString, queryString, parameters);
                command.Connection.Open();
                DbTransaction dbTransaction;
                using var transaction = dbTransaction = command.Connection.BeginTransaction();
                command.Transaction = dbTransaction;
                var rowsAffected = 0;
                foreach (var timeSheet in timeSheetList)
                {
                    command.Parameters["@UserId"].Value = timeSheet.UserId ;
                    command.Parameters["@Project"].Value = timeSheet.Project;
                    command.Parameters["@Epic"].Value = timeSheet.Epic ?? (object)DBNull.Value;
                    command.Parameters["@Feature"].Value = timeSheet.Feature ?? (object)DBNull.Value;
                    command.Parameters["@UserStory"].Value = timeSheet.UserStory ?? (object)DBNull.Value;
                    command.Parameters["@Requirements"].Value = timeSheet.Requirements ?? (object)DBNull.Value;
                    command.Parameters["@Task"].Value = timeSheet.Task ?? (object)DBNull.Value;
                    command.Parameters["@TimeSheetDate"].Value = timeSheet.TimeSheetDate;
                    command.Parameters["@TimeSheetHours"].Value = timeSheet.TimeSheetHours;

                    rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                        break;
                }

                if (rowsAffected > 0)
                {
                    dbTransaction.Commit();
                }
                else
                {
                    errorDO.Id = (int) ErrorEnum.DatabaseSaveWeekTimeSheetError;
                    errorDO.Message =
                        ErrorEnum.DatabaseSaveWeekTimeSheetError.GetDescription();
                    dbTransaction.Rollback();
                }

                command.Connection.Close();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"{ErrorEnum.DatabaseSaveWeekTimeSheetError.GetDescription()} Error Message {ex.Message}",
                    ErrorEnum.DatabaseSaveWeekTimeSheetError, ex);
            }

            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogExitingMethodInfo,
                    $"{typeof(TimeSheetRepo)}_{MethodBase.GetCurrentMethod()}"));

            return errorDO;
        }
        public WeekTimeInfoDO SaveTimeSheet(WeekTimeInfoDO timeSheet)
        {
            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogEnteringMethodInfo,
                    $"{typeof(TimeSheetRepo)}_{MethodBase.GetCurrentMethod()}"));
            if (timeSheet == null)
                //Get the method name
                throw new ApplicationException(
                    $" {MethodBase.GetCurrentMethod()} is null in {typeof(TimeSheetRepo)}_{MethodBase.GetCurrentMethod()} Please check");
            var errorDO = new ErrorDO();
            try
            {
                var queryString = RepoConstants.SQL_INSERT_WEEKTIMESHEET;
                var parameters = new Dictionary<string, object>
                {
                    {"@UserId", SqlDbType.NVarChar},
                    {"@Project", SqlDbType.NVarChar},
                    {"@Epic", SqlDbType.NVarChar},
                    {"@Feature", SqlDbType.NVarChar},
                    {"@UserStory", SqlDbType.NVarChar},
                    {"@Requirements", SqlDbType.NVarChar},
                    {"@Task", SqlDbType.NVarChar},
                    {"@TimeSheetDate", SqlDbType.DateTime},
                    {"@TimeSheetHours", SqlDbType.Int}
                };
                using var command = SqlConnectionManager.GetDbCommand(_connectionString, queryString, parameters);
                command.Connection.Open();
                DbTransaction dbTransaction;
                using var transaction = dbTransaction = command.Connection.BeginTransaction();
                command.Transaction = dbTransaction;
                
                command.Parameters["@UserId"].Value = timeSheet.UserId;
                command.Parameters["@Project"].Value = timeSheet.Project;
                command.Parameters["@Epic"].Value = timeSheet.Epic ?? (object)DBNull.Value; 
                command.Parameters["@Feature"].Value = timeSheet.Feature ?? (object)DBNull.Value; 
                command.Parameters["@UserStory"].Value = timeSheet.UserStory ?? (object)DBNull.Value ;
                command.Parameters["@Requirements"].Value = timeSheet.Requirements ?? (object)DBNull.Value; 
                command.Parameters["@Task"].Value = timeSheet.Task;
                command.Parameters["@TimeSheetDate"].Value = timeSheet.TimeSheetDate;
                command.Parameters["@TimeSheetHours"].Value = timeSheet.TimeSheetHours;

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    command.CommandText = "select top 1 id from tblTimeSheet order by id desc";
                    var timeSheetId = (int)command.ExecuteScalar();
                    timeSheet.TimeSheetId = timeSheetId;
                    dbTransaction.Commit();
                }
                else
                {
                    
                    errorDO.Id = (int) ErrorEnum.DatabaseSaveTimeSheetError;
                    errorDO.Message =
                        ErrorEnum.DatabaseSaveTimeSheetError.GetDescription();
                    dbTransaction.Rollback();
                    timeSheet.ErrorListDo.Add(errorDO);
                }

                command.Connection.Close();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"{ErrorEnum.DatabaseSaveTimeSheetError.GetDescription()} Error Message {ex.Message}",
                    ErrorEnum.DatabaseSaveTimeSheetError, ex);
            }

            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogExitingMethodInfo,
                    $"{typeof(TimeSheetRepo)}_{MethodBase.GetCurrentMethod()}"));

            return timeSheet;
        }

        public ErrorDO UpdateTimeSheet(WeekTimeInfoDO timeSheet)
        {
            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogEnteringMethodInfo,
                    $"{typeof(TimeSheetRepo)}_{MethodBase.GetCurrentMethod()}"));
            if (timeSheet == null)
                //Get the method name
                throw new ApplicationException(
                    $" {MethodBase.GetCurrentMethod()} is null in {typeof(TimeSheetRepo)}_{MethodBase.GetCurrentMethod()} Please check");
            var errorDO = new ErrorDO();
            try
            {
                var queryString = RepoConstants.SQL_UPDATE_WEEKTIMESHEET;
                var parameters = new Dictionary<string, object>
                {
                    {"@TimeSheetHours", SqlDbType.Int},
                    {"@Task", SqlDbType.NVarChar},
                    {"@TimeSheetDate", SqlDbType.DateTime}
                };
                using var command = SqlConnectionManager.GetDbCommand(_connectionString, queryString, parameters);
                command.Connection.Open();
                DbTransaction dbTransaction;
                using var transaction = dbTransaction = command.Connection.BeginTransaction();
                command.Transaction = dbTransaction;

                command.Parameters["@TimeSheetHours"].Value = timeSheet.TimeSheetHours;
                command.Parameters["@Task"].Value = timeSheet.Task;
                command.Parameters["@TimeSheetDate"].Value = timeSheet.TimeSheetDate;

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    dbTransaction.Commit();
                }
                else
                {
                    errorDO.Id = (int)ErrorEnum.DatabaseUpdateTimeSheetError;
                    errorDO.Message =
                        ErrorEnum.DatabaseUpdateTimeSheetError.GetDescription();
                    dbTransaction.Rollback();
                }

                command.Connection.Close();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"{ErrorEnum.DatabaseUpdateTimeSheetError.GetDescription()} Error Message {ex.Message}",
                    ErrorEnum.DatabaseUpdateTimeSheetError, ex);
            }

            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogExitingMethodInfo,
                    $"{typeof(TimeSheetRepo)}_{MethodBase.GetCurrentMethod()}"));

            return errorDO;
        }

        public List<WeekTimeInfoDO> GetWeekTimeInfoList(string userID)
        {
            var timeSheetList = new  List<WeekTimeInfoDO>();
            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogEnteringMethodInfo,
                    $"{typeof(TimeSheetRepo)}_{MethodBase.GetCurrentMethod()}"));
            try
            {
                var queryString = RepoConstants.SQL_GET_TIMESHEET_BY_USER_ID;
                var parameters = new Dictionary<string, object>
                {
                    { "@param1", userID }
                };

                using var command = SqlConnectionManager.GetDbCommand(_connectionString, queryString, parameters);
                command.Connection.Open();
                using var dataReader = new SafeDataReader(command.ExecuteReader());
                while (dataReader.Read())
                {
                    timeSheetList.Add( new WeekTimeInfoDO()
                    {
                        Epic = dataReader.GetString("Epic"),
                        Feature = dataReader.GetString("Feature"),
                        UserStory = dataReader.GetString("UserStory"),
                        Requirements = dataReader.GetString("Requirements"),
                        Project = dataReader.GetString("Project"),
                        Task = dataReader.GetString("Task"),
                        TimeSheetHours = dataReader.GetInt32("TimeSheetHours"),
                        TimeSheetDate = dataReader.GetDateTime("TimeSheetDate")
                    });
                }
                command.Connection.Close();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"{ErrorEnum.DataGetTimeSheetError.GetDescription()} Error Message {ex.Message}",
                    ErrorEnum.DataGetTimeSheetError, ex);
            }
            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogExitingMethodInfo,
                    $"{typeof(TimeSheetRepo)}_{MethodBase.GetCurrentMethod()}"));

            return timeSheetList;
        }


        public List<WeekTimeSheetDO> GetWeekTimeByPivotDay(string userID)
        {
            var timeSheetList = new List<WeekTimeSheetDO>();
            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogEnteringMethodInfo,
                    $"{typeof(TimeSheetRepo)}_{MethodBase.GetCurrentMethod()}"));
            try
            {
                var queryString = RepoConstants.SQL_GET_WEEKTIMESHEET_PIVOT_BY_DAY;
                var parameters = new Dictionary<string, object>
                {
                    { "@param1", userID }
                };

                using var command = SqlConnectionManager.GetDbCommand(_connectionString, queryString, parameters);
                command.Connection.Open();
                using var dataReader = new SafeDataReader(command.ExecuteReader());
                while (dataReader.Read())
                {
                    timeSheetList.Add(new WeekTimeSheetDO()
                    {
                        Epic = dataReader.GetString("Epic"),
                        Feature = dataReader.GetString("Feature"),
                        UserStory = dataReader.GetString("UserStory"),
                        Requirement = dataReader.GetString("Requirements"),
                        Project = dataReader.GetString("Project"),
                        Task = dataReader.GetString("Task"),
                        WeekEndDate = dataReader.GetDateTime("WeekEndDate"),
                        WeekStartDate = dataReader.GetDateTime("WeekStartDate"),
                        Sunday = dataReader.GetInt32("Sunday"),
                        Monday = dataReader.GetInt32("Monday"),
                        Tuesday = dataReader.GetInt32("Tuesday"),
                        Wednesday = dataReader.GetInt32("Wednesday"),
                        Thursday = dataReader.GetInt32("Thursday"),
                        Friday = dataReader.GetInt32("Friday"),
                        Saturday = dataReader.GetInt32("Saturday"),
                        Total = dataReader.GetInt32("total")

                    });
                }
                command.Connection.Close();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"{ErrorEnum.DataGetTimeSheetError.GetDescription()} Error Message {ex.Message}",
                    ErrorEnum.DataGetTimeSheetError, ex);
            }
            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogExitingMethodInfo,
                    $"{typeof(TimeSheetRepo)}_{MethodBase.GetCurrentMethod()}"));

            return timeSheetList;
        }

        public ErrorDO DeleteTimeSheet(string task,DateTime timeSheetDateTime)
        {
            
            var errorDO = new ErrorDO();
            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogEnteringMethodInfo,
                    $"{typeof(TimeSheetRepo)}_{MethodBase.GetCurrentMethod()}"));
            try
            {
                var queryString = RepoConstants.SQL_DELETE_TIMESHEET_BY_TIMESHEET_ID;
                var parameters = new Dictionary<string, object>
                {
                    {"@param1", task},
                    {"@param2", timeSheetDateTime}


                };
                using var command = SqlConnectionManager.GetDbCommand(_connectionString, queryString, parameters);
                command.Connection.Open();
                DbTransaction dbTransaction;
                using var transaction = dbTransaction = command.Connection.BeginTransaction();
                command.Transaction = dbTransaction;
                var rowsAffected = command.ExecuteNonQuery();
                dbTransaction.Commit();

                if (rowsAffected <= 0)
                {
                    errorDO.Id = (int) ErrorEnum.DataDeleteTimeSheetError;
                    errorDO.Message = "Unable to delete TimeSheer(s)";

                }
                command.Connection.Close();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"{ErrorEnum.DataDeleteTimeSheetError.GetDescription()} Error Message {ex.Message}",
                    ErrorEnum.DataDeleteTimeSheetError, ex);
            }

            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogExitingMethodInfo,
                    $"{typeof(TimeSheetRepo)}_{MethodBase.GetCurrentMethod()}"));

            return new ErrorDO();
        }

        #endregion
    }
}