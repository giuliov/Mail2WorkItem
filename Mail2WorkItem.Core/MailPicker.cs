using OpenPop.Mime;
using OpenPop.Pop3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mail2WorkItem.Core
{
    public class MailPicker
    {
        string hostname;
        int port;
        bool useSsl;
        string username;
        string password;
        string tempFolder;

        public MailPicker(string hostname, int port, bool useSsl, string username, string password, string tempFolder)
        {
            this.hostname = hostname;
            this.port = port;
            this.useSsl = useSsl;
            this.username = username;
            this.password = password;
            this.tempFolder = tempFolder;
        }

        public IList<MailMessage> ReadAll(string tempFolder)
        {
            var result = new List<MailMessage>();

            using (Pop3Client client = new Pop3Client())
            {
                client.Connect(hostname, port, useSsl);
                client.Authenticate(username, password);

                try
                {
                    int num = client.GetMessageCount();

                    for (int i = 1; i <= num; i++)
                    {
                        var pop3mail = client.GetMessage(i);

                        var message = MakeMessage(pop3mail);
                        SaveAttachments(pop3mail, message);

                        result.Add(message);
#if !DEBUG
                        client.DeleteMessage(i);
#endif
                    }//for

                    // also forces message deletion
                    client.Disconnect();
                }
                catch (Exception e)
                {
                    //TODO log
                }//try

                return result;
            }//using
        }

        private MailMessage MakeMessage(Message pop3mail)
        {
            // favor HTML
            MessagePart body = pop3mail.FindFirstHtmlVersion();
            if (body == null)
            {
                body = pop3mail.FindFirstPlainTextVersion();
            }

            string tempFile = Path.ChangeExtension(Path.Combine(tempFolder, pop3mail.Headers.MessageId), ".eml");
            var fi = new FileInfo(tempFile);
            pop3mail.Save(fi);

            var message = new MailMessage()
            {
                Body = body.GetBodyAsText(),
                Subject = pop3mail.Headers.Subject,
                From = pop3mail.Headers.From ?? pop3mail.Headers.Sender,
                MessageId = pop3mail.Headers.MessageId,
                LocalDumpFile = tempFile
            };

            return message;
        }

        private void SaveAttachments(Message pop3mail, MailMessage message)
        {
            string mailTempFolder = Path.Combine(tempFolder, pop3mail.Headers.MessageId);
            Directory.CreateDirectory(mailTempFolder);
            foreach (var pop3attachment in pop3mail.FindAllAttachments())
            {
                string tempFile = Path.Combine(mailTempFolder, pop3attachment.FileName);
                var fi = new FileInfo(tempFile);
                pop3attachment.Save(fi);
                var attachment = new MailAttachment()
                {
                    Name = pop3attachment.FileName,
                    LocalDumpFile = tempFile
                };
                message.Attachments.Add(attachment);
            }//for
        }
    }
}
