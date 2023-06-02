using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Nilsa
{
    internal class PersoneVkAttributes
    {
        private string _sex;
        public PersoneVkAttributes(long personId, string firstName, string lastName, string relation, string birthDay, 
            string city, string country, string countersFriends, string online, string lastSeen)
        {
            PersonId = personId;
            FirstName = firstName;
            LastName = lastName;
            Relation = relation;
            BirthDate = birthDay;
            City = city;
            Country = country;
            CountersFriends = countersFriends;
            Online = online;
            LastSeen = lastSeen;
        }
        public long PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Relation { get; set; }
        public string BirthDate { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountersFriends { get; set; }
        public string Online { get; set; }
        public string LastSeen { get; set; }
        public string Sex
        {
            get { return _sex; }
            set
            {
                if (LastSeen.ToLower().StartsWith("заходил ")) { _sex = "Мужской"; }
                else if (LastSeen.ToLower().StartsWith("заходила ")) { _sex = "Женский"; }
            } }
    }
}
