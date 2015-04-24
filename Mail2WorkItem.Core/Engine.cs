using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.IO;

namespace Mail2WorkItem.Core
{
    public class Engine
    {
        EngineConfiguration configuration;

        public Engine(EngineConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Run()
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), "Mail2WorkItem");
            Directory.CreateDirectory(tempFolder);

            try
            {
                var picker = new MailPicker(
                        configuration.Pop3.Hostname, configuration.Pop3.Port, configuration.Pop3.UseSsl,
                        configuration.Pop3.Username, configuration.Pop3.Password,
                        tempFolder);
                var messages = picker.ReadAll(tempFolder);

                var tfs = new TfsFacade(new Uri(configuration.TFS.CollectionUrl), configuration.TFS.ProjectName);
                foreach (var mail in messages)
                {
                    int workItemId = tfs.AddWorkItem(mail, configuration.TFS.WorkItemType);
                    SendConfirmation(mail, workItemId);
                }//for
            }
            finally
            {
                Directory.Delete(tempFolder, true);
            }//try
        }

        private void SendConfirmation(MailMessage mail, int workItemId)
        {
            // TODO make easier customization
            var buf = new StringBuilder();
            buf.AppendFormat("{0} #{1} has been recorded on your behalf"
                , configuration.TFS.WorkItemType
                , workItemId
                );
            buf.AppendLine();

            var message = new System.Net.Mail.MailMessage();
            message.From = new MailAddress(configuration.Smtp.From);
            message.To.Add(new MailAddress(mail.From.Address, mail.From.DisplayName));
            message.Subject = "Request received";
            message.Body = buf.ToString();

            var smtp = new SmtpClient(configuration.Smtp.Hostname, configuration.Smtp.Port);
            smtp.EnableSsl = configuration.Smtp.UseSsl;
            smtp.Credentials = new System.Net.NetworkCredential(configuration.Smtp.Username, configuration.Smtp.Password);

            smtp.Send(message);
        }
    }
}
