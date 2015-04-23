using OpenPop.Mime;
using OpenPop.Pop3;
using System;
using System.Collections.Generic;
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

        public MailPicker(string hostname, int port, bool useSsl, string username, string password)
        {
            this.hostname = hostname;
            this.port = port;
            this.useSsl = useSsl;
            this.username = username;
            this.password = password;
        }

        public IList<MailMessage> ReadAll()
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
                        var mail = client.GetMessage(i);

                        // favor HTML
                        MessagePart body = mail.FindFirstHtmlVersion();
                        if (body == null)
                        {
                            body = mail.FindFirstPlainTextVersion();
                        }

                        var msg = new MailMessage()
                        {
                            Body = body.GetBodyAsText(),
                            Subject = mail.Headers.Subject,
                            From = mail.Headers.From ?? mail.Headers.Sender
                        };
                        result.Add(msg);

                        client.DeleteMessage(i);
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
    }
}
