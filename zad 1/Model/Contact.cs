using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zad_1.Model
{
    public class Contact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address Address { get; set; }
        public List<Telephone> Telephones { get; set; }
        public Contact()
        {
            Address = new Address();
            Telephones = new List<Telephone>();
        }


    }
}
