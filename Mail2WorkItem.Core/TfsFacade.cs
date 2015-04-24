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
        ILogEvents logger;
        private TfsTeamProjectCollection tfs;
        private WorkItemStore workItemStore;
        private string projectName;
        private string workItemTypeName;

        public TfsFacade(ILogEvents logger, Uri tfsCollectionUri, string projectName, string workItemTypeName)
        {
            this.logger = logger;
            this.projectName = projectName;
            this.workItemTypeName = workItemTypeName;

            this.tfs = new TfsTeamProjectCollection(tfsCollectionUri);
            logger.ConnectingToTfs(tfsCollectionUri);
            this.tfs.EnsureAuthenticated();
            this.workItemStore = tfs.GetService<WorkItemStore>();
            logger.ConnectedToTfs(tfsCollectionUri);

            if (!workItemStore.Projects.Contains(this.projectName))
            {
                logger.TfsProjectNotFound(this.projectName);
            }
            if (!workItemStore.Projects[projectName].WorkItemTypes.Contains(workItemTypeName))
            {
                logger.TfsWorkItemTypeNotFound(projectName, workItemTypeName);
            }
        }

        public int AddWorkItem(MailMessage mail)
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
                int existingId = queryResult[0].Id;
                logger.AWorkItemAlreadyExistsFor(existingId, mail);
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

            if (!workItem.IsValid())
            {
                logger.InvalidWorkItem(workItem);
            }
            else
            {
                logger.SavingWorkItem(workItem);
                workItem.Save();
                logger.WorkItemSaved(workItem);
            }

            return workItem.Id;
        }
    }
}
