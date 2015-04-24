using OpenPop.Mime.Header;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mail2WorkItem.Core
{
    public class MailMessage
    {
        public string Body { get; set; }
        public string Subject { get; set; }
        public RfcMailAddress From { get; set; }
        public string MessageId { get; set; }
        public string LocalDumpFile { get; set; }
        public List<MailAttachment> Attachments { get; protected set; }

        public MailMessage()
        {
            Attachments = new List<MailAttachment>();
        }
    }

    public class MailAttachment
    {
        public string Name { get; set; }
        public string LocalDumpFile { get; set; }
    }
}
