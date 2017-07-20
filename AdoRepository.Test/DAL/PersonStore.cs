using AdoRepository.Test.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoRepository.Test.DAL
{
    public class PersonStore : AdoGenericRepository.AdoRepository<Person>
    {
        public PersonStore(string connectionString) 
            : base(Defs.Configs.ConnectionString)
        {

        }

        override 
    }
}
