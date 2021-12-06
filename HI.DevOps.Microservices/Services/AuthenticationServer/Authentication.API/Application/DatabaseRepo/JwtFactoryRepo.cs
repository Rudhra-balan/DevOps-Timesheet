using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using Hi.DevOps.Authentication.API.Application.Constants;
using Hi.DevOps.Authentication.API.Application.DatabaseRepoInterface;
using Hi.DevOps.Authentication.API.Common;
using Hi.DevOps.Authentication.API.Common.Enum;
using Hi.DevOps.Authentication.API.DataObject;
using HI.DevOps.DatabaseContext.ConnectionManager;
using HI.DevOps.DatabaseContext.ConnectionManager.SafeDataReader;
using log4net;
using MySql.Data.MySqlClient;
using ApplicationException = Hi.DevOps.Authentication.API.Common.Exception.ApplicationException;

namespace Hi.DevOps.Authentication.API.Application.DatabaseRepo
{
    public class JwtFactoryRepo : IJwtFactoryRepo
    {
        #region constructor

        public JwtFactoryRepo(string connectionString)
        {
            _connectionString = connectionString;
            SysLog = LogManager.GetLogger(typeof(JwtFactoryRepo));
        }

        #endregion

        #region Private

        private readonly string _connectionString;
        public ILog SysLog { get; }

        #endregion

        #region public

        public User GetUserInformation(string emailAddress)
        {
            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogEnteringMethodInfo,
                    $"{typeof(JwtFactoryRepo)}_{MethodBase.GetCurrentMethod()}"));
            if (emailAddress.IsNullOrEmpty())
                //Get the method name
                throw new ApplicationException(
                    $" {MethodBase.GetCurrentMethod()} is null in {typeof(JwtFactoryRepo)}_{MethodBase.GetCurrentMethod()} Please check");
            var userInfo = new User();
            try
            {
                var queryString = RepoConstants.SqlGetUser;
                var parameters = new Dictionary<string, object> {{"@param1", emailAddress}};
                using var command = SqlConnectionManager.GetDbCommand(_connectionString, queryString, parameters);
                command.Connection.Open();
                using var dataReader = new SafeDataReader(command.ExecuteReader());
                while (dataReader.Read())
                {
                    userInfo.UserIdentityId = dataReader.GetGuid("User_Unique_id").ToString();
                    userInfo.UserRole = (UserRoleEnum)dataReader.GetInt32("UserRole");
                    userInfo.IsActive = dataReader.GetBoolean("IsActive");
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting the user id",
                    ErrorEnum.DatabaseUserGettingUserIdError, ex);
            }

            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogExitingMethodInfo,
                    $"{typeof(JwtFactoryRepo)}_{MethodBase.GetCurrentMethod()}"));
            return userInfo;
        }
        public bool UpdateRefreshToken(string token)
        {
            var isRefreshTTokenUpdate = false;
            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogEnteringMethodInfo,
                    $"{typeof(JwtFactoryRepo)}_{MethodBase.GetCurrentMethod()}"));
            if (token.IsNullOrEmpty())
                //Get the method name
                throw new ApplicationException(
                    $" {MethodBase.GetCurrentMethod()} is null in {typeof(JwtFactoryRepo)}_{MethodBase.GetCurrentMethod()} Please check");
            try
            {
                var queryString = RepoConstants.SqlGenerateRefreshToken;
                var parameters = new Dictionary<string, object>
                {
                    {"@iRefreshToken", token},
                    {"@oMessage", MySqlDbType.VarChar},
                    {"@oIsValidRefreshToken", MySqlDbType.Bit}
                };
                using var command =
                    SqlConnectionManager.GetDbCommand(_connectionString, queryString, parameters, true, 255);
                command.Connection.Open();
                using var dataReader = new SafeDataReader(command.ExecuteReader());
                while (dataReader.Read())
                    if (!dataReader.GetString("Message").IsNullOrEmpty() ||
                        dataReader.GetString("Message").Contains("Success"))
                        isRefreshTTokenUpdate = dataReader.GetBoolean("IsValidRefreshToken");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while validating the user",
                    ErrorEnum.DatabaseUserAuthenticateError, ex);
            }

            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogExitingMethodInfo,
                    $"{typeof(JwtFactoryRepo)}_{MethodBase.GetCurrentMethod()}"));
            return isRefreshTTokenUpdate;
        }
       
        #endregion

    }
}