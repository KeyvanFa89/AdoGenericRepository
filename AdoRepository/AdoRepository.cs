using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AdoGenericRepository
{
    public enum CommandResults
    {
        AffectedRows = 1,
        ReturnValue  = 2
    }
    public abstract class AdoRepository<T> where T : class
    {
        #region Fields

        private SqlConnection _connection;
        private SqlCommand _command;
        #endregion

        #region ctors

        public AdoRepository(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _command = new SqlCommand();
        }

        #endregion

        #region MustImplement
        public abstract T MapToModel(SqlDataReader reader);

        #endregion

        protected SqlParameter CreateSqlParamter<K>(string name,K value)
        {
            return new SqlParameter(name, (object)value ?? DBNull.Value);
        }
        protected async Task<IEnumerable<T>> GetRecordsAsync(string tSql,params SqlParameter[] sqlParameteres)
        {
            var list = new List<T>();

            _command.Connection = _connection;
            _command.CommandText = tSql;

            if (sqlParameteres != null && sqlParameteres.Count() > 0)
                _command.Parameters.AddRange(sqlParameteres);

            try
            {
                _connection.Open();
                var reader = await _command.ExecuteReaderAsync();
                try
                {
                    while (await reader.ReadAsync())
                        list.Add(MapToModel(reader));
                }
                finally
                {
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _connection.Close();
            }
            return list;
        }
        protected async Task<T> GetRecordAsync(string tSql,params SqlParameter[] sqlParameteres)
        {
            T record = null;

            _command.Connection = _connection;
            _command.CommandText = tSql;

            if (sqlParameteres != null && sqlParameteres.Count() > 0)
                _command.Parameters.AddRange(sqlParameteres);

            try
            {
                _connection.Open();
                var reader = await _command.ExecuteReaderAsync();
                try
                {
                    while (await reader.ReadAsync())
                    {
                        record = MapToModel(reader);
                        break;
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _connection.Close();
            }
            return record;
        }
        public async Task<dynamic> ExcecuteCommandAsync(string cmd,CommandResults resultType,params SqlParameter[] sqlParameters)
        {
            _command.Connection = _connection;
            _command.CommandText = cmd;

            if (sqlParameters != null && sqlParameters.Count() > 0)
                _command.Parameters.AddRange(sqlParameters);

            try
            {
                _connection.Open();
                switch (resultType)
                {
                    case CommandResults.AffectedRows:
                        return await _command.ExecuteNonQueryAsync();
                        
                    case CommandResults.ReturnValue:
                        return await _command.ExecuteScalarAsync();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _connection.Close();
            }
        }

    }

}
