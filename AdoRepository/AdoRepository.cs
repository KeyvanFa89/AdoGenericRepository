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
        public abstract T MapToModel(SqlDataReader reader);

        public abstract SqlParameter[] CreateInsertSqlParameters(T entity);
        public abstract SqlParameter[] CreateUpdateSqlParameters(T entity);
        public abstract SqlParameter CreateSelectByIdParameter<K>(K id);

        public abstract string GetInsertCommand();
        public abstract string GetUpdateCommand();
        public abstract string GetSelectByIdCommand();
        public abstract string GetSelectAllCommand();
        #endregion

        #region Implemented
        protected SqlParameter CreateSqlParamter<K>(string name, K value)
        {
            return new SqlParameter(name, (object)value ?? DBNull.Value);
        }
        #endregion

        #region CRUD_Operations

        public async Task<object> Insert(T entity)
        {
            object res;

            try
            {
                using (_connection = new SqlConnection())
                {
                    _connection.Open();
                    using (_command = new SqlCommand(GetInsertCommand(), _connection))
                    {
                        _command.CommandType = System.Data.CommandType.Text;
                        _command.Parameters.AddRange(CreateInsertSqlParameters(entity));

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
                using (_connection = new SqlConnection())
                {
                    _connection.Open();
                    using (_command = new SqlCommand(GetUpdateCommand(), _connection))
                    {
                        _command.CommandType = System.Data.CommandType.Text;
                        _command.Parameters.AddRange(CreateUpdateSqlParameters(entity));

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
