using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Classlibrary;

namespace Practica01.Models
{
    public class Person
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("dpi")]
        public string dpi { get; set; }
        [JsonProperty("datebirth")]
        public string datebirth { get; set; }
        [JsonProperty("address")]
        public string address { get; set; }
        public string[] companies { get; set; }

        public string[] encode;

        public Comparison<Person> nameComparer = delegate (Person person1, Person person2)
        {
            return person1.name.CompareTo(person2.name);
        };

        public Comparison<Person> dpiComparer = delegate (Person person1, Person person2)
        {
            return person1.dpi.CompareTo(person2.dpi);
        };

        public static int dataEncode (Person person1, string company)
        {
            if (person1.companies.Contains(company))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        public static Person PatchData(Person person1, Person person2)
        {
            Person resultante = new Person();
            person2.name = person1.name;
            person2.dpi = person1.dpi;
            person2.address = person1.address;
            person2.datebirth = person1.datebirth;
            resultante = person2;
            return resultante;
        }
    }
    
}
