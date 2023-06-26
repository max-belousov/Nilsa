using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilsa.SelfLearning
{
    public class MessageInList
    {
        public MessageInList() { }
        public string ZEROS { get; set; }
        public string SET { get; set; }
        public string TITLE { get; set; }
        public string STADIA_OBSHENIA { get; set; }
        public string NUMBER_IN_DIALOG { get; set; }
        public string VARIANT { get; set; }
        public string SUBVARIANT { get; set; }
        public string CONT_TO_PERS { get; set; }
        public string PERS_TO_CONT { get; set; }
        public string ROLE_GROUP { get; set; }
        public string INTERESTS { get; set; }
        public string ROLE_COMUNICATION { get; set; }
        public string TYPE_MSG { get; set; }
        public string SEX_PERS { get; set; }
        public string SEX_CONT { get; set; }
        public string IGNORE { get; set; }
        public string ID { get; set; }
        public string TEXT { get; set; }
        public string MARKER { get; set; }

        public void ParseMessageFields(string line)
        {
            string[] fields = line.Split('|');
            ZEROS = fields[0];
            SET = fields[1];
            TITLE = fields[2];
            STADIA_OBSHENIA = fields[3];
            NUMBER_IN_DIALOG = fields[4];
            VARIANT = fields[5];
            SUBVARIANT = fields[6];
            CONT_TO_PERS = fields[7];
            PERS_TO_CONT = fields[8];
            ROLE_GROUP = fields[9];
            INTERESTS = fields[10];
            ROLE_COMUNICATION = fields[11];
            TYPE_MSG = fields[12];
            SEX_PERS = fields[13];
            SEX_CONT = fields[14];
            IGNORE = fields[15];
            ID = fields[16];
            TEXT = fields[17];
            if (fields.Length == 18) MARKER = "";
            else MARKER = fields[18];
        }
        public void FillPropertiesFromList(List<string> values)
        {
            ZEROS = "000000";
            SET = values[0];
            TITLE = values[1];
            STADIA_OBSHENIA = values[2];
            NUMBER_IN_DIALOG = values[3];
            VARIANT = values[4];
            SUBVARIANT = values[5];
            CONT_TO_PERS = values[6];
            PERS_TO_CONT = values[7];
            ROLE_GROUP = values[8];
            INTERESTS = values[9];
            ROLE_COMUNICATION = values[10];
            TYPE_MSG = values[11];
            SEX_PERS = values[12];
            SEX_CONT = values[13];
            IGNORE = values[14];
            ID = values[15];
            TEXT = "";
            MARKER = ""; // Empty string for MARKER
        }
        public override string ToString()
        {
            if (String.IsNullOrWhiteSpace(MARKER)) return $"{ZEROS}|{SET}|{TITLE}|{STADIA_OBSHENIA}|{NUMBER_IN_DIALOG}|{VARIANT}|{SUBVARIANT}|{CONT_TO_PERS}|{PERS_TO_CONT}|{ROLE_GROUP}|{INTERESTS}|{ROLE_COMUNICATION}|{TYPE_MSG}|{SEX_PERS}|{SEX_CONT}|{IGNORE}|{ID}|@!{TEXT}";
            else return $"{ZEROS}|{SET}|{TITLE}|{STADIA_OBSHENIA}|{NUMBER_IN_DIALOG}|{VARIANT}|{SUBVARIANT}|{CONT_TO_PERS}|{PERS_TO_CONT}|{ROLE_GROUP}|{INTERESTS}|{ROLE_COMUNICATION}|{TYPE_MSG}|{SEX_PERS}|{SEX_CONT}|{IGNORE}|{ID}|@!{TEXT}|{MARKER}";
        }
    }

}
