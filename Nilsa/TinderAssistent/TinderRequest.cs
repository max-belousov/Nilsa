using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilsa.TinderAssistent
{
    internal class TinderRequest
    {
        public TinderRequest(string mode, string command, string personePhone, string tinderId, string charName = "")
        {
            Mode = mode;
            Command = command;
            PersonePhone = personePhone;
            TinderId = tinderId;
            CharacteristicName = charName;
        }
        public string Mode { get; set; }
        public string Command { get; set; }
        public string PersonePhone { get; set; }
        public string TinderId { get; set; }
        public string CharacteristicName { get; set; }
        public string AuthorisationCode { get; set; }
    }
}
