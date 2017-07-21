using AdoGenericRepository;
using AdoRepository.Test.Entities;
using System;
using System.Data.SqlClient;

namespace AdoRepository.Test.DAL
{
    public class PersonStore : AdoRepository<Person>
    {
        #region ctor
        public PersonStore()
            : base(Defs.Configs.ConnectionString)
        {
        }
        #endregion

        private const string INSERT_COMMAND =
            @"INSERT INTO [School].[dbo].[Persons]([FirstName],[LastName],[Height])
            VALUES(@FirstName, @LastName, @Height)";

        private const string UPDATE_COMMAND =
            @"UPDATE [School].[dbo].[Persons]
               SET [FirstName] = @FirstName,
                   [LastName]  = @LastName, 
                   [Height]    = @Height
               WHERE [Id] = @Id";

        private const string SELECT_BY_ID_COMMAND =
            @"SELECT TOP 1 * FROM [School].[dbo].[Persons] WHERE [Id] = @Id";

        protected override SqlParameter[] MapToSqlParameters(Person entity)
        {
            SqlParameter[] insertSqlParameters =
            {
                CreateSqlParamter("Id",entity.Id),
                CreateSqlParamter("FirstName",entity.FirstName),
                CreateSqlParamter("LastName",entity.LastName),
                CreateSqlParamter("Height",entity.Height)
            };

            return insertSqlParameters;
        }

        protected override string GetInsertCommand()
        {
            return INSERT_COMMAND;
        }

        protected override string GetSelectAllCommand()
        {
            throw new NotImplementedException();
        }

        protected override string GetSelectByIdCommand()
        {
            return SELECT_BY_ID_COMMAND;
        }

        protected override string GetUpdateCommand()
        {
            return UPDATE_COMMAND;
        }

        protected override Person MapToModel(SqlDataReader reader)
        {
            Person model = new Person
            {
                Id        = IsDBNull(reader,"Id",0),       
                FirstName = IsDBNull(reader,"FirstName",""),
                LastName  = IsDBNull(reader,"LastName",""), 
                Height    = IsDBNull(reader,"Height",0) 
            };
            return model;
        }

        

        protected override SqlParameter CreateSelectByIdParameter<K>(K id)
        {
            return CreateSqlParamter("Id", id);
        }
    }
}
