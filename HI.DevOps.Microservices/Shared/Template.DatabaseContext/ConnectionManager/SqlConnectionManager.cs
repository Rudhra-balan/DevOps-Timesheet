using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace HI.DevOps.DatabaseContext.ConnectionManager
{
    public static class SqlConnectionManager
    {
        #region Protected Methods

        /// <summary>
        ///     Gets or sets a <see cref="System.String" /> that represents the
        ///     connection string to connect to a database.
        /// </summary>
        private static string ConnectionString { get; }

        #endregion

        #region Private Method

        private static void SetCommandParameter(SqlCommand command, object value, string key, int size)
        {
            switch ((SqlDbType) value)
            {
                case SqlDbType.VarChar:
                    command.Parameters.Add(key, SqlDbType.VarChar, size);
                    command.Parameters[key].Direction = ParameterDirection.Output;
                    break;
                case SqlDbType.Bit:
                    command.Parameters.Add(key, SqlDbType.Bit);
                    command.Parameters[key].Direction = ParameterDirection.Output;
                    break;
                case SqlDbType.Int:
                    command.Parameters.Add(key, SqlDbType.Int);
                    command.Parameters[key].Direction = ParameterDirection.Output;
                    break;
            }
        }

        #endregion

        #region Public Methods
        
        public static DbCommand GetDbCommand(string connectionString, string queryString,
            Dictionary<string, object> parameters, bool storedProcedure, int size = 100)
        {
            var connection = new SqlConnection(connectionString);
            var command = new SqlCommand(queryString, connection);
            if (storedProcedure)
                command.CommandType = CommandType.StoredProcedure;

            if (parameters == null) throw new ArgumentException("Parameters are null");

            foreach (var parameter in parameters)
                if (parameter.Value.GetType() == typeof(SqlDbType))
                    SetCommandParameter(command, parameter.Value, parameter.Key, size);
                else
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);

            return command;
        }


        /// <summary>
        ///     Gets a <see cref="System.Data.Common.DbCommand" />
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="queryString"> SQL statement </param>
        /// <param name="parameters">Dictionary key value pair of parameters</param>
        /// <returns>
        ///     <see cref="System.Data.Common.DbCommand" />
        /// </returns>
        public static DbCommand GetDbCommand(string connectionString, string queryString,
            Dictionary<string, object> parameters)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
            var connection = new SqlConnection(connectionString);
            var command = new SqlCommand(queryString, connection);
            try
            {
                if (parameters == null) throw new ArgumentException("Parameters are null");

                foreach (var parameter in parameters)
                {
                    if (parameter.Value.GetType() == typeof(SqlDbType))
                        command.Parameters.Add(parameter.Key, (SqlDbType) parameter.Value);
                    else command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return command;
        }

        public static DbCommand GetDbCommand(string connectionString, string queryString, DataTable inputTable,
            string tableKey)
        {
            var connection = new SqlConnection(ConnectionString);
            var command = new SqlCommand(queryString, connection);
            if (inputTable == null) throw new ArgumentException("Parameters are null");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue(tableKey, inputTable);
            return command;
        }

        public static DbCommand GetDbCommand(string connectionString, string queryString, List<string> parameters)
        {
            var connection = new SqlConnection(ConnectionString);
            var command = new SqlCommand(queryString, connection);

            if (parameters == null) throw new ArgumentException("Parameters are null");

            foreach (var parameterName in parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = parameterName;
                command.Parameters.Add(parameter);
            }

            return command;
        }

        /// <summary>
        ///     Gets a <see cref="System.Data.Common.DbCommand" />
        /// </summary>
        /// <param name="queryString"> SQL statement </param>
        /// <returns>
        ///     <see cref="System.Data.Common.DbCommand" />
        /// </returns>
        public static DbCommand GetDbCommand(string queryString)
        {
            var connection = new SqlConnection(ConnectionString);
            var command = new SqlCommand(queryString, connection);

            return command;
        }

        /// <summary>
        ///     Gets a <see cref="System.Data.Common.DbCommand" />
        /// </summary>
        /// <param name="queryString"> SQL statement </param>
        /// <param name="connectionString"></param>
        /// <returns>
        ///     <see cref="System.Data.Common.DbCommand" />
        /// </returns>
        public static DbCommand GetDbCommand(string connectionString, string queryString)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
            var connection = new SqlConnection(connectionString);
            var command = new SqlCommand(queryString, connection);

            return command;
        }

        /// <summary>
        ///     Executes the ExecuteNonQuery operation for a given query string and parameters
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="queryString"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static bool ExecuteNonQuery(string connectionString, string queryString,
            Dictionary<string, object> parameters)
        {
            var isSaveSuccess = false;

            using (var command = GetDbCommand(ConnectionString, queryString, parameters))
            {
                try
                {
                    command.Connection.Open();
                    var recordsAffected = command.ExecuteNonQuery();

                    if (recordsAffected >= 0) isSaveSuccess = true;
                }
                catch (Exception)
                {
                    command.Connection.Close();

                    throw;
                }
            }

            return isSaveSuccess;
        }

        #endregion
    }
}