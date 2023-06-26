using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilsa.Data
{
    public class Contacter
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhotoURL { get; set; }

        public Contacter(int id, string firstName, string lastName, string photoURL)
        {
            ID = id;
            FirstName = firstName;
            LastName = lastName;
            PhotoURL = photoURL;
        }

        public static Contacter Parse(string input)
        {
            string[] parts = input.Split('|');
            if (parts.Length != 3)
                throw new ArgumentException("Invalid input format.");

            int id = int.Parse(parts[0]);
            var name = parts[1].Split(' ');
            string photoURL = parts[2];
            var firstName = "";
            var lastName = "";
            if (name.Length == 2)
            {
                firstName = name[0];
                lastName = name[1];
            }
            else
            {
                firstName = name[0];
            }


            return new Contacter(id, firstName, lastName, photoURL);
        }

        public override string ToString()
        {
            return $"{ID}|{FirstName} {LastName}|{PhotoURL}";
        }
    }
}
