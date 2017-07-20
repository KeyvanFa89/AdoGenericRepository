using AdoRepository.Test.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using AdoGenericRepository;

namespace AdoRepository.Test.DAL
{
    public class PersonStore : AdoGenericRepository.AdoRepository<Person>
    {
        public PersonStore(string connectionString)
            : base(connectionString)
        {

        }

        public PersonStore()
            : base(Defs.Configs.ConnectionString)
        {

        }

        private const string Table = "Persons";

        public override Person MapToModel(SqlDataReader reader)
        {
            return new Person
            {
                Id = (reader["Id"] as int?) ?? -1,
                FirstName = (reader["FirstName"] as string) ?? "",
                LastName = (reader["LastName"] as string) ?? ""
            };
        }

        public async Task<Person> GetAsync(int id)
        {
            try
            {
                string tSql = $"SELECT * FROM {Table} WHERE Id = @Id";
                Person p = await this.GetRecordAsync(tSql, new SqlParameter("Id", id));
                return p;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> AddAsync(Person person)
        {
            try
            {
                string insertCommand =
                    @"INSERT INTO [School].[dbo].[Persons]([FirstName],[LastName],[Height])
                    OUTPUT inserted.Id
                    VALUES(@FirstName, @LastName, @Height)";

                SqlParameter[] parameters =
                {
                    CreateSqlParamter("FirstName",person.FirstName),
                    CreateSqlParamter("LastName",person.LastName),
                    CreateSqlParamter("Height",person.Height)
                };

                return await this.ExcecuteCommandAsync
                    (insertCommand, CommandResults.ReturnValue, parameters);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
