using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mail2WorkItem.Core
{
    public enum LogLevel
    {
        Critical = 1,
        Error = 2,
        Warning = 3,
        Information = 5,
        Normal = Information,
        Verbose = 10,
        Diagnostic = 99,
    }

    public interface ILogEvents
    {
        void ConnectingToPop3Server(string hostname);
        void ConnectedToPop3Server(string hostname);
        void FoundMessages(int numMessages);
        void ReadingMessage(int messageNo);
        void MessageRead(OpenPop.Mime.Message pop3mail);
        void DisconnectingFromPop3Server(string hostname);
        void DisconnectedFromPop3Server(string hostname);
        void ErrorWhileReadingFromPop3Server(string hostname, Exception e);
        void SavingIncomingMailTo(string tempFile);
        void SavedIncomingMailTo(string tempFile);
        void SavingIncomingMailAttachmentTo(string tempFile);
        void SavedIncomingMailAttachmentTo(string tempFile);
        void ConnectingToTfs(Uri tfsCollectionUri);
        void ConnectedToTfs(Uri tfsCollectionUri);
        void TfsProjectNotFound(string p);
        void TfsWorkItemTypeNotFound(string projectName, string workItemTypeName);
        void AWorkItemAlreadyExistsFor(int existingId, MailMessage mail);
        void InvalidWorkItem(Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem workItem);
        void SavingWorkItem(Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem workItem);
        void WorkItemSaved(Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem workItem);
        void FailToSendConfirmationMessage(System.Net.Mail.MailMessage message, Exception e);
        void RemovingTemporaryFolder(string tempFolder);

        void UnexpectedError(Exception e);
    }
}
