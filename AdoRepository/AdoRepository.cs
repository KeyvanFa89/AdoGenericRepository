using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AdoGenericRepository
{
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

        public virtual T MapToModel(SqlDataReader reader)
        {
            return null;
        }
        public virtual  SqlParameter[] MapToSqlParameters<T>(T model)
        {
            return null;
        }
        protected async Task<IEnumerable<T>> GetRecordsAsync(string tSql, SqlParameter[] sqlParameteres)
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
        protected async Task<T> GetRecordAsync(string tSql, SqlParameter[] sqlParameteres)
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
        public async Task<object> ExcecuteCommand(string cmd,SqlParameter[] sqlParameters)
        {
            _command.Connection = _connection;
            _command.CommandText = cmd;

            if (sqlParameters != null && sqlParameters.Count() > 0)
                _command.Parameters.AddRange(sqlParameters);

            try
            {
                return await _command.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

}
