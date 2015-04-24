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
        ILogEvents logger;
        string hostname;
        int port;
        bool useSsl;
        string username;
        string password;
        string tempFolder;

        public MailPicker(ILogEvents logger, string hostname, int port, bool useSsl, string username, string password, string tempFolder)
        {
            this.logger = logger;
            this.hostname = hostname;
            this.port = port;
            this.useSsl = useSsl;
            this.username = username;
            this.password = password;
            this.tempFolder = tempFolder;
        }

        public IList<MailMessage> ReadAll()
        {
            var result = new List<MailMessage>();

            logger.ConnectingToPop3Server(this.hostname);

            using (Pop3Client client = new Pop3Client())
            {
                client.Connect(hostname, port, useSsl);
                client.Authenticate(username, password);

                logger.ConnectedToPop3Server(this.hostname);

                try
                {
                    int numMessages = client.GetMessageCount();
                    logger.FoundMessages(numMessages);

                    for (int messageNo = 1; messageNo <= numMessages; messageNo++)
                    {
                        logger.ReadingMessage(messageNo);
                        var pop3mail = client.GetMessage(messageNo);
                        logger.MessageRead(pop3mail);

                        var message = MakeMessage(pop3mail);
                        SaveAttachments(pop3mail, message);

                        result.Add(message);
#if !DEBUG
                        logger.DeletingMessage(messageNo);
                        client.DeleteMessage(messageNo);
#endif
                    }//for

                    logger.DisconnectingFromPop3Server(this.hostname);
                    // also forces message deletion
                    client.Disconnect();
                    logger.DisconnectedFromPop3Server(this.hostname);
                }
                catch (Exception e)
                {
                    logger.ErrorWhileReadingFromPop3Server(this.hostname, e);
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

            // .elm is usually associated with default email client
            string tempFile = Path.Combine(tempFolder, pop3mail.Headers.MessageId) + ".eml";
            logger.SavingIncomingMailTo(tempFile);
            var fi = new FileInfo(tempFile);
            pop3mail.Save(fi);
            logger.SavedIncomingMailTo(tempFile);

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
                logger.SavingIncomingMailAttachmentTo(tempFile);
                var fi = new FileInfo(tempFile);
                pop3attachment.Save(fi);
                logger.SavedIncomingMailAttachmentTo(tempFile);
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
