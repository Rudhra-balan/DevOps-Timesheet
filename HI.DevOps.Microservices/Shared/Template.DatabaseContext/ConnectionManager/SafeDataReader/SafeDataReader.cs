using System;
using System.Data;

namespace HI.DevOps.DatabaseContext.ConnectionManager.SafeDataReader
{
    /// <summary>
    ///     This is a DataReader that 'fixes' any null values before
    ///     they are returned to our business code.
    /// </summary>
    public class SafeDataReader : IDataReader
    {
        #region Protected Methods

        /// <summary>
        ///     Get a reference to the underlying data reader
        ///     object that actually contains the data from
        ///     the data source.
        /// </summary>
        protected IDataReader DataReader { get; }

        #endregion

        #region Private Members

        #endregion

        #region Public Methods

        /// <summary>
        ///     Initializes the SafeDataReader object to use data from
        ///     the provided DataReader object.
        /// </summary>
        public SafeDataReader(IDataReader dataReader)
        {
            DataReader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));
        }

        /// <summary>
        ///     Determines whether a field with the specified name exists in
        ///     the reader or not.
        /// </summary>
        /// <param name="name">
        ///     A <see cref="System.String" /> that represents the name of the
        ///     field to lookup.
        /// </param>
        /// <returns>
        ///     A <see cref="System.Boolean" /> value indicating whether the field
        ///     exists or not.
        /// </returns>
        public bool HasField(string name)
        {
            for (var i = 0; i < DataReader.FieldCount; i++)
                if (DataReader.GetName(i).Equals(name,
                    StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }

        /// <summary>
        ///     Gets a string value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns empty string for null.
        /// </remarks>
        public string GetString(string name)
        {
            return GetString(DataReader.GetOrdinal(name));
        }

        /// <summary>
        ///     Gets a string value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns empty string for null.
        /// </remarks>
        public virtual string GetString(int i)
        {
            if (DataReader.IsDBNull(i))
                return string.Empty;
            return DataReader.GetString(i);
        }
        /// <summary>
        ///     Gets a value of type <see cref="System.Object" /> from the data reader.
        /// </summary>
        public object GetValue(string name)
        {
            return GetValue(DataReader.GetOrdinal(name));
        }

        /// <summary>
        ///     Gets a value of type <see cref="System.Object" /> from the data reader.
        /// </summary>
        public virtual object GetValue(int i)
        {
            if (DataReader.IsDBNull(i))
                return null;
            return DataReader.GetValue(i);
        }

        /// <summary>
        ///     Gets an integer from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns 0 for null.
        /// </remarks>
        public int GetInt32(string name)
        {
            return GetInt32(DataReader.GetOrdinal(name));
        }

        /// <summary>
        ///     Gets nullable integer from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns 0 for null.
        /// </remarks>
        public int? GetInt32Null(string name)
        {
            return GetInt32Null(DataReader.GetOrdinal(name));
        }

        /// <summary>
        ///     Gets an integer from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns 0 for null.
        /// </remarks>
        public virtual int GetInt32(int i)
        {
            if (DataReader.IsDBNull(i))
                return 0;
            return DataReader.GetInt32(i);
        }

        /// <summary>
        ///     Gets nullable integer from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns null.
        /// </remarks>
        public virtual int? GetInt32Null(int i)
        {
            if (DataReader.IsDBNull(i))
                return null;
            return DataReader.GetInt32(i);
        }

        /// <summary>
        ///     Gets a double from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns 0 for null.
        /// </remarks>
        public double GetDouble(string name)
        {
            return GetDouble(DataReader.GetOrdinal(name));
        }

        /// <summary>
        ///     Gets a double from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns 0 for null.
        /// </remarks>
        public virtual double GetDouble(int i)
        {
            if (DataReader.IsDBNull(i))
                return 0;
            return DataReader.GetDouble(i);
        }

        /// <summary>
        ///     Gets a Guid value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns Guid.Empty for null.
        /// </remarks>
        public Guid GetGuid(string name)
        {
            return GetGuid(DataReader.GetOrdinal(name));
        }

        /// <summary>
        ///     Gets a Guid value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns Guid.Empty for null.
        /// </remarks>
        public virtual Guid GetGuid(int i)
        {
            return DataReader.IsDBNull(i) ? Guid.Empty : DataReader.GetGuid(i);
        }

        /// <summary>
        ///     Reads the next row of data from the data reader.
        /// </summary>
        public bool Read()
        {
            return DataReader.Read();
        }

        /// <summary>
        ///     Moves to the next result set in the data reader.
        /// </summary>
        public bool NextResult()
        {
            return DataReader.NextResult();
        }

        /// <summary>
        ///     Moves to the next row in the data reader.
        /// </summary>
        public bool NextRow()
        {
            return DataReader.Read();
        }

        /// <summary>
        ///     Closes the data reader.
        /// </summary>
        public void Close()
        {
            DataReader.Close();
        }

        /// <summary>
        ///     Returns the depth property value from the data reader.
        /// </summary>
        public int Depth => DataReader.Depth;

        /// <summary>
        ///     Returns the FieldCount property from the data reader.
        /// </summary>
        public int FieldCount => DataReader.FieldCount;

        /// <summary>
        ///     Gets a boolean value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns <see langword="false" /> for null.
        /// </remarks>
        public bool GetBoolean(string name)
        {
            return GetBoolean(DataReader.GetOrdinal(name));
        }

        /// <summary>
        ///     Gets a boolean value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns <see langword="false" /> for null.
        /// </remarks>
        public virtual bool GetBoolean(int i)
        {
            return !DataReader.IsDBNull(i) && DataReader.GetBoolean(i);
        }

        /// <summary>
        ///     Gets a byte value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns 0 for null.
        /// </remarks>
        public byte GetByte(string name)
        {
            return GetByte(DataReader.GetOrdinal(name));
        }

        /// <summary>
        ///     Gets a byte value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns 0 for null.
        /// </remarks>
        public virtual byte GetByte(int i)
        {
            return DataReader.IsDBNull(i) ? (byte)0 : DataReader.GetByte(i);
        }

        /// <summary>
        ///     Invokes the GetBytes method of the underlying data reader.
        /// </summary>
        /// <remarks>
        ///     Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        /// <param name="fieldOffset"></param>
        /// <param name="bufferoffset"></param>
        /// <param name="length"></param>
        /// <param name="buffer"></param>
        public long GetBytes(string name, long fieldOffset,
            byte[] buffer, int bufferoffset, int length)
        {
            return buffer.Length == 0
                ? throw new ArgumentException("Value cannot be an empty collection.", nameof(buffer))
                : GetBytes(DataReader.GetOrdinal(name), fieldOffset, buffer, bufferoffset, length);
        }

        /// <summary>
        ///     Invokes the GetBytes method of the underlying data reader.
        /// </summary>
        /// <remarks>
        ///     Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        /// <param name="fieldOffset"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferoffset"></param>
        /// <param name="length"></param>
        public virtual long GetBytes(int i, long fieldOffset,
            byte[] buffer, int bufferoffset, int length)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (buffer.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(buffer));
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));
            if (DataReader.IsDBNull(i))
                return 0;
            return DataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        /// <summary>
        ///     Gets a char value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns Char.MinValue for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public char GetChar(string name)
        {
            return GetChar(DataReader.GetOrdinal(name));
        }

        /// <summary>
        ///     Gets a char value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns Char.MinValue for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual char GetChar(int i)
        {
            if (DataReader.IsDBNull(i))
                return char.MinValue;
            var myChar = new char[1];
            DataReader.GetChars(i, 0, myChar, 0, 1);
            return myChar[0];
        }

        /// <summary>
        ///     Invokes the GetChars method of the underlying data reader.
        /// </summary>
        /// <remarks>
        ///     Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        /// <param name="fieldoffset"></param>
        /// <param name="bufferoffset"></param>
        /// <param name="length"></param>
        /// <param name="buffer"></param>
        public long GetChars(string name, long fieldoffset,
            char[] buffer, int bufferoffset, int length)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (buffer.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(buffer));
            return GetChars(DataReader.GetOrdinal(name), fieldoffset, buffer, bufferoffset, length);
        }

        /// <summary>
        ///     Invokes the GetChars method of the underlying data reader.
        /// </summary>
        /// <remarks>
        ///     Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        /// <param name="fieldoffset"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferoffset"></param>
        /// <param name="length"></param>
        public virtual long GetChars(int i, long fieldoffset,
            char[] buffer, int bufferoffset, int length)
        {
            if (DataReader.IsDBNull(i))
                return 0;
            return DataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        /// <summary>
        ///     Invokes the GetData method of the underlying data reader.
        /// </summary>
        /// <param name="name">Name of the column containing the value.</param>
        public IDataReader GetData(string name)
        {
            return GetData(DataReader.GetOrdinal(name));
        }

        /// <summary>
        ///     Invokes the GetData method of the underlying data reader.
        /// </summary>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual IDataReader GetData(int i)
        {
            return DataReader.GetData(i);
        }

        /// <summary>
        ///     Invokes the GetDataTypeName method of the underlying data reader.
        /// </summary>
        /// <param name="name">Name of the column containing the value.</param>
        public string GetDataTypeName(string name)
        {
            return GetDataTypeName(DataReader.GetOrdinal(name));
        }

        /// <summary>
        ///     Invokes the GetDataTypeName method of the underlying data reader.
        /// </summary>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual string GetDataTypeName(int i)
        {
            return DataReader.GetDataTypeName(i);
        }

        /// <summary>
        ///     Gets a date value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns DateTime.MinValue for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public virtual DateTime GetDateTime(string name)
        {
            return GetDateTime(DataReader.GetOrdinal(name));
        }

        /// <summary>
        ///     Gets a date value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns DateTime.MinValue for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual DateTime GetDateTime(int i)
        {
            return DataReader.IsDBNull(i) ? DateTime.MinValue : DataReader.GetDateTime(i);
        }

        /// <summary>
        ///     Gets a decimal value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public decimal GetDecimal(string name)
        {
            return GetDecimal(DataReader.GetOrdinal(name));
        }

        /// <summary>
        ///     Gets a decimal value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual decimal GetDecimal(int i)
        {
            return DataReader.IsDBNull(i) ? 0 : DataReader.GetDecimal(i);
        }

        /// <summary>
        ///     Invokes the GetFieldType method of the underlying data reader.
        /// </summary>
        /// <param name="name">Name of the column containing the value.</param>
        public Type GetFieldType(string name)
        {
            return GetFieldType(DataReader.GetOrdinal(name));
        }

        /// <summary>
        ///     Invokes the GetFieldType method of the underlying data reader.
        /// </summary>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual Type GetFieldType(int i)
        {
            return DataReader.GetFieldType(i);
        }

        /// <summary>
        ///     Gets a Single value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public float GetFloat(string name)
        {
            return GetFloat(DataReader.GetOrdinal(name));
        }

        /// <summary>
        ///     Gets a Single value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual float GetFloat(int i)
        {
            return DataReader.IsDBNull(i) ? 0 : DataReader.GetFloat(i);
        }

        /// <summary>
        ///     Gets a Short value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public short GetInt16(string name)
        {
            return GetInt16(DataReader.GetOrdinal(name));
        }

        /// <summary>
        ///     Gets a Short value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual short GetInt16(int i)
        {
            return DataReader.IsDBNull(i) ? (short)0 : DataReader.GetInt16(i);
        }

        /// <summary>
        ///     Gets a Long value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public long GetInt64(string name)
        {
            return GetInt64(DataReader.GetOrdinal(name));
        }

        /// <summary>
        ///     Gets a Long value from the data reader.
        /// </summary>
        /// <remarks>
        ///     Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual long GetInt64(int i)
        {
            return DataReader.IsDBNull(i) ? 0 : DataReader.GetInt64(i);
        }

        /// <summary>
        ///     Invokes the GetName method of the underlying data reader.
        /// </summary>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual string GetName(int i)
        {
            return DataReader.GetName(i);
        }

        /// <summary>
        ///     Gets an ordinal value from the data reader.
        /// </summary>
        /// <param name="name">Name of the column containing the value.</param>
        public int GetOrdinal(string name)
        {
            return DataReader.GetOrdinal(name);
        }

        /// <summary>
        ///     Invokes the GetSchemaTable method of the underlying data reader.
        /// </summary>
        public DataTable GetSchemaTable()
        {
            return DataReader.GetSchemaTable();
        }


        /// <summary>
        ///     Invokes the GetValues method of the underlying data reader.
        /// </summary>
        public int GetValues(object[] values)
        {
            return DataReader.GetValues(values);
        }

        /// <summary>
        ///     Returns the IsClosed property value from the data reader.
        /// </summary>
        public bool IsClosed => DataReader.IsClosed;

        /// <summary>
        ///     Invokes the IsDBNull method of the underlying data reader.
        /// </summary>
        /// <param name="i"></param>
        public virtual bool IsDBNull(int i)
        {
            return DataReader.IsDBNull(i);
        }

        /// <summary>
        ///     Returns a value from the data reader.
        /// </summary>
        /// <param name="name">Name of the column containing the value.</param>
        public object this[string name]
        {
            get
            {
                var val = DataReader[name];
                return DBNull.Value.Equals(val) ? null : val;
            }
        }

        /// <summary>
        ///     Returns a value from the data reader.
        /// </summary>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual object this[int i]
        {
            get
            {
                if (DataReader.IsDBNull(i))
                    return null;
                return DataReader[i];
            }
        }

        /// <summary>
        ///     Returns the RecordsAffected property value from the underlying data reader.
        /// </summary>
        public int RecordsAffected => DataReader.RecordsAffected;

        #endregion

        #region IDisposable Support

        private bool _disposedValue; // To detect redundant calls

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
                if (disposing)
                    // free unmanaged resources when explicitly called
                    DataReader.Dispose();

            // free shared unmanaged resources
            _disposedValue = true;
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}