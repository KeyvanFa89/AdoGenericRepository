using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoRepository.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            DAL.PersonStore personStore = new DAL.PersonStore();
            var person = personStore.GetAsync(4).Result;
            Console.WriteLine($"{person.Id} - {person.FirstName} - {person.LastName}");
            Console.ReadKey();
        }
    }
}
