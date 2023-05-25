using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Nilsa.TinderAssistent
{
    public class TinderResponse
    {
        public TinderResponse() { }
        public int STATUS{ get; set; }
        public string MESSAGE{ get; set; }
        public string TEXT { get; set; }
        public IncomeMessage[] DATA { get; set; }
        public long ID { get; set; }
        public string URL { get; set; }
        public string PATH { get; set; }
        public string FIRST_NAME_PERSONE { get; set; }
        public string LAST_NAME_PERSONE { get; set; }
        public string FIRST_NAME_CONTACTER { get; set; }
        public string LAST_NAME_CONTACTER { get; set; }
        public string CONTACTER { get; set; }
        public string COMMAND { get; set; }
        public IncomeMessage[] NEW_MESSAGES { get; set; }
        public UnreadMessage[] UNREAD_MESSAGES { get; set;  }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (STATUS != 0)
            {
                builder.Append("STATUS: ").AppendLine(STATUS.ToString());
            }

            if (!string.IsNullOrEmpty(MESSAGE))
            {
                builder.Append("MESSAGE: ").AppendLine(MESSAGE);
            }

            if (!string.IsNullOrEmpty(TEXT))
            {
                builder.Append("TEXT: ").AppendLine(TEXT);
            }

            if (DATA != null && DATA.Length > 0)
            {
                builder.Append("DATA:");
                builder.AppendLine();
                foreach (var item in DATA)
                {
                    builder.AppendLine(item.ToString());
                }
            }

            if (ID != 0)
            {
                builder.Append("ID: ").AppendLine(ID.ToString());
            }

            if (!string.IsNullOrEmpty(URL))
            {
                builder.Append("URL: ").AppendLine(URL);
            }

            if (!string.IsNullOrEmpty(PATH))
            {
                builder.Append("PATH: ").AppendLine(PATH);
            }

            if (!string.IsNullOrEmpty(FIRST_NAME_PERSONE))
            {
                builder.Append("FIRST_NAME_PERSONE: ").AppendLine(FIRST_NAME_PERSONE);
            }

            if (!string.IsNullOrEmpty(LAST_NAME_PERSONE))
            {
                builder.Append("LAST_NAME_PERSONE: ").AppendLine(LAST_NAME_PERSONE);
            }

            if (!string.IsNullOrEmpty(FIRST_NAME_CONTACTER))
            {
                builder.Append("FIRST_NAME_CONTACTER: ").AppendLine(FIRST_NAME_CONTACTER);
            }

            if (!string.IsNullOrEmpty(LAST_NAME_CONTACTER))
            {
                builder.Append("LAST_NAME_CONTACTER: ").AppendLine(LAST_NAME_CONTACTER);
            }

            if (!string.IsNullOrEmpty(CONTACTER))
            {
                builder.Append("CONTACTER: ").AppendLine(CONTACTER);
            }

            if (!string.IsNullOrEmpty(COMMAND))
            {
                builder.Append("COMMAND: ").AppendLine(COMMAND);
            }

            if (NEW_MESSAGES != null && NEW_MESSAGES.Length > 0)
            {
                builder.Append("NEW_MESSAGES:");
                builder.AppendLine();
                foreach (var item in NEW_MESSAGES)
                {
                    builder.AppendLine(item.ToString());
                }
            }

            if (UNREAD_MESSAGES != null && UNREAD_MESSAGES.Length > 0)
            {
                builder.Append("UNREAD_MESSAGES:");
                builder.AppendLine();
                foreach (var item in UNREAD_MESSAGES)
                {
                    builder.AppendLine(item.ToString());
                }
            }

            return builder.ToString().TrimEnd();
        }
    }

    
}
