using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilsa.Data
{
    internal class PersoneAllData //класс для сериализации в JSON и обработки всех характеристик персонажа
    {
        public PersoneAllData() { }

        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Operators { get; set; }
        public string Login { get; set; }
        public string Sex { get; set; }
        public string City { get; set; }
        public string Age { get; set; }
        public string FamilyStatus { get; set; }
        public string FriendsCount { get; set; }
        public string CommunicationRole { get; set; }
        public string SelfDescription { get; set; }
        public string WorkHours { get; set; }
        public string Owner { get; set; }
        public string Group { get; set; }
        public string Project { get; set; }
        public string Source { get; set; }
        public string Password { get; set; }
        public string BaseAlgorithm { get; set; }
        public string Status { get; set; }
    }
}
