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
        ReturnValue = 2
    }
    public abstract class AdoRepository<T> where T : class
    {
        #region Fields

        private SqlConnection _connection;
        private SqlCommand _command;
        private readonly string _connectionString;
        #endregion

        #region ctors

        public AdoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion

        #region MustImplement
        protected abstract T MapToModel(SqlDataReader reader);

        protected abstract SqlParameter[] MapToSqlParameters(T entity);
        protected abstract SqlParameter CreateSelectByIdParameter<K>(K id);

        protected abstract string GetInsertCommand();
        protected abstract string GetUpdateCommand();
        protected abstract string GetSelectByIdCommand();
        protected abstract string GetSelectAllCommand();
        #endregion

        #region Implemented
        protected SqlParameter CreateSqlParamter<K>(string name, K value)
        {
            return new SqlParameter(name, (object)value ?? DBNull.Value);
        }
        protected dynamic IsDBNull(SqlDataReader reader, string fieldName, object defaultValue)
        {
            return reader[fieldName] == DBNull.Value ? defaultValue : (dynamic)reader[fieldName];
        }
        protected async Task<List<T>> Query(string tSql,params SqlParameter[] sqlParameters)
        {
            List<T> lstEntities = new List<T>();

            try
            {
                using (_connection = new SqlConnection(_connectionString))
                {
                    _connection.Open();
                    using (_command = new SqlCommand(tSql, _connection))
                    {
                        _command.CommandType = System.Data.CommandType.Text;

                        if (sqlParameters != null && sqlParameters.Count() > 0)
                            _command.Parameters.AddRange(sqlParameters);

                        var reader = await _command.ExecuteReaderAsync();
                        if (reader.HasRows)
                            while (await reader.ReadAsync())
                                lstEntities.Add(MapToModel(reader));
                    }
                }

                return lstEntities;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                _connection.Close();
                _command.Dispose();
            }
        }
        protected async Task<int> ExexuteCommand(string command,params SqlParameter[] sqlParameters)
        {
            int rowsAffected = 0;

            try
            {
                using (_connection = new SqlConnection(_connectionString))
                {
                    _connection.Open();
                    using (_command = new SqlCommand(command, _connection))
                    {
                        _command.CommandType = System.Data.CommandType.Text;

                        if (sqlParameters != null && sqlParameters.Count() > 0)
                            _command.Parameters.AddRange(sqlParameters);

                        rowsAffected = await _command.ExecuteNonQueryAsync();
                        
                    }
                }

                return rowsAffected;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                _connection.Close();
                _command.Dispose();
            }
        }

        #endregion

        #region CRUD_Operations_Depend_On_Implement_All_Abstract_Methods

        public async Task<object> Insert(T entity)
        {
            object res;

            try
            {
                using (_connection = new SqlConnection(_connectionString))
                {
                    _connection.Open();
                    using (_command = new SqlCommand(GetInsertCommand(), _connection))
                    {
                        _command.CommandType = System.Data.CommandType.Text;
                        _command.Parameters.AddRange(MapToSqlParameters(entity));

                        res = await _command.ExecuteScalarAsync();
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _connection.Close();
                _command.Dispose();
            }
        }

        public async Task<int> Update(T entity)
        {
            int rowsAffected = 0;

            try
            {
                using (_connection = new SqlConnection(_connectionString))
                {
                    _connection.Open();
                    using (_command = new SqlCommand(GetUpdateCommand(), _connection))
                    {
                        _command.CommandType = System.Data.CommandType.Text;
                        _command.Parameters.AddRange(MapToSqlParameters(entity));

                        rowsAffected = await _command.ExecuteNonQueryAsync();
                    }
                }

                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _connection.Close();
                _command.Dispose();
            }
        }

        public async Task<T> GetById<K>(K id)
        {
            try
            {
                using (_connection = new SqlConnection(_connectionString))
                {
                    _connection.Open();
                    using (_command = new SqlCommand(GetSelectByIdCommand(), _connection))
                    {
                        _command.Parameters.Add(CreateSelectByIdParameter(id));
                        _command.CommandType = System.Data.CommandType.Text;

                        var reader = await _command.ExecuteReaderAsync();

                        if (reader.HasRows)
                            while (await reader.ReadAsync())
                                return MapToModel(reader);
                    }
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
                _command.Dispose();
            }
        }

        public async Task<List<T>> GetAll()
        {
            List<T> lstEntities = new List<T>();

            try
            {
                using (_connection = new SqlConnection(_connectionString))
                {
                    _connection.Open();
                    using (_command = new SqlCommand(GetSelectAllCommand(), _connection))
                    {
                        _command.CommandType = System.Data.CommandType.Text;

                        var reader = await _command.ExecuteReaderAsync();

                        if (reader.HasRows)
                            while (reader.Read())
                                lstEntities.Add(MapToModel(reader));

                        return lstEntities;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                _connection.Close();
                _command.Dispose();
            }
        }

        #endregion

    }

}
