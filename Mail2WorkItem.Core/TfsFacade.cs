using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
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

            var workItem = new WorkItem(workItemType);
            workItem.Open();
            workItem.Title = mail.Subject;
            workItem.Description = mail.Body;
            workItem.Save();

            return workItem.Id;
        }
    }
}
