using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mail2WorkItem.Core
{
    public class TfsFacade
    {
        private TfsTeamProjectCollection tfs;
        private WorkItemStore workItemStore;
        private string projectName;

        public TfsFacade(Uri tfsCollectionUri, string projectName)
        {
            this.projectName = projectName;

            this.tfs = new TfsTeamProjectCollection(tfsCollectionUri);
            this.workItemStore = tfs.GetService<WorkItemStore>();
        }

        public int AddWorkItem(MailMessage mail, string workItemTypeName)
        {
            var workItemType = workItemStore.Projects[projectName].WorkItemTypes[workItemTypeName];

            string wiql = string.Format(
@"SELECT [System.Id], [System.WorkItemType], [System.Title]
FROM WorkItems
WHERE [System.TeamProject] = '{0}'
AND  [System.WorkItemType] = '{1}'
AND  [System.Title] = '{2}'
AND  [System.HyperLinkCount] >= 1
AND  [System.AttachedFileCount] >= 1"
                , projectName
                , workItemTypeName
                , mail.Subject);
            var queryResult = workItemStore.Query(wiql);

            if (queryResult.Count > 0)
            {
                //LOG
                return 0;
            }

            var workItem = new WorkItem(workItemType);
            workItem.Open();
            workItem.Title = mail.Subject;
            workItem.Description = mail.Body;
            var link = new Hyperlink("mailto:" + mail.MessageId);
            workItem.Links.Add(link);

            Attachment originalMailAsAttachment = new Attachment(mail.LocalDumpFile, "Original email as attachment");
            workItem.Attachments.Add(originalMailAsAttachment);

            foreach (var mailAttachment in mail.Attachments)
            {
                Attachment newAttachment = new Attachment(mailAttachment.LocalDumpFile, "Attachment from email");
                workItem.Attachments.Add(newAttachment);
            }//for

            workItem.Save();

            return workItem.Id;
        }
    }
}
