using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mail2WorkItem.Core
{
    public class Pop3Configuration
    {
        public string Hostname { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class TfsConfiguration
    {
        public string CollectionUrl { get; set; }
        public string ProjectName { get; set; }
        public string WorkItemType { get; set; }
    }

    public class SmtpConfiguration
    {
        public string Hostname { get; set; }
        public int Port { get; set; }
        public string From { get; set; }
        public bool UseSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public SmtpConfiguration()
        {
            this.Port = 25;
        }
    }

    public class EngineConfiguration
    {
        public Pop3Configuration Pop3 { get; protected set; }
        public TfsConfiguration TFS { get; protected set; }
        public SmtpConfiguration Smtp { get; protected set; }

        public EngineConfiguration()
        {
            this.Pop3 = new Pop3Configuration();
            this.TFS = new TfsConfiguration();
            this.Smtp = new SmtpConfiguration();
        }
    }
}
