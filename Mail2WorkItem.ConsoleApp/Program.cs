using Mail2WorkItem.ConsoleApp.Properties;
using Mail2WorkItem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mail2WorkItem.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new EngineConfiguration();
            config.Pop3.Hostname = Settings.Default.POP3_Hostname;
            config.Pop3.Port = Settings.Default.POP3_Port;
            config.Pop3.UseSsl = Settings.Default.POP3_UseSsl;
            config.Pop3.Username = Settings.Default.POP3_Username;
            config.Pop3.Password = Settings.Default.POP3_Password;
            config.TFS.CollectionUrl = Settings.Default.TFS_CollectionUrl;
            config.TFS.ProjectName = Settings.Default.TFS_ProjectName;
            config.TFS.WorkItemType = Settings.Default.TFS_WorkItemType;
            config.Smtp.Hostname = Settings.Default.SMTP_Hostname;
            config.Smtp.Port = Settings.Default.SMTP_Port;
            config.Smtp.From = Settings.Default.SMTP_From;

            var engine = new Engine(config);
            engine.Run();
        }
    }
}
