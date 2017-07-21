using AdoRepository.Test.Entities;
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
            select();
        }


        static void select()
        {
            DAL.PersonStore personStore = new DAL.PersonStore();
            var person = personStore.GetById(13).Result;
            Console.WriteLine(person.FirstName);
            Console.ReadLine();
        }

        static async void insert()
        {
            DAL.PersonStore personStore = new DAL.PersonStore();
            var person = new Person
            {
                FirstName = "آرمین",
                LastName  = "سامانی",
                Height = 168,
                Id = 2
            };
            var res = await personStore.Insert(person);
            Console.WriteLine((int)res);
            Console.ReadKey();
        }

        static async void update()
        {
            DAL.PersonStore personStore = new DAL.PersonStore();
            var person = new Person
            {
                FirstName = "آرمین",
                LastName = "آقا سامانی",
                Height = 168,
                Id = 14
            };
            var res = await personStore.Update(person);
            Console.WriteLine((int)res);
            Console.ReadKey();
        }
    }
}
