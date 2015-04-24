using Mail2WorkItem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mail2WorkItem.ConsoleApp
{
    class ConsoleEventLogger : LoggerBase, ILogEvents
    {
        public ConsoleEventLogger(LogLevel level)
            : base(level)
        { }

        public void ConnectingToPop3Server(string hostname)
        {
            Log(LogLevel.Verbose, "Connecting to POP3 server '{0}'", hostname);
        }

        public void ConnectedToPop3Server(string hostname)
        {
            Log(LogLevel.Information, "Connected to POP3 server '{0}'", hostname);
        }

        public void FoundMessages(int numMessages)
        {
            Log(LogLevel.Verbose, "Found {0} mail messages", numMessages);
        }

        public void ReadingMessage(int messageNo)
        {
            Log(LogLevel.Verbose, "Reading POP3 message #{0}", messageNo);
        }

        public void MessageRead(OpenPop.Mime.Message pop3mail)
        {
            Log(LogLevel.Information, "Read POP3 message '{0}' from '{1}' [id {2}]"
                , pop3mail.Headers.Subject, pop3mail.Headers.From, pop3mail.Headers.MessageId);
        }

        public void DisconnectingFromPop3Server(string hostname)
        {
            Log(LogLevel.Verbose, "Disconnecting from POP3 server '{0}'", hostname);
        }

        public void DisconnectedFromPop3Server(string hostname)
        {
            Log(LogLevel.Information, "Disconnected from POP3 server '{0}'", hostname);
        }

        public void ErrorWhileReadingFromPop3Server(string hostname, Exception e)
        {
            Log(LogLevel.Critical, "Error connecting/processing messages from '{0}'\nException: {1}", hostname, e.Message);
        }

        public void SavingIncomingMailTo(string tempFile)
        {
            Log(LogLevel.Verbose, "Saving incoming mail to '{0}'", tempFile);
        }

        public void SavedIncomingMailTo(string tempFile)
        {
            Log(LogLevel.Information, "Saved incoming mail to '{0}'", tempFile);
        }

        public void SavingIncomingMailAttachmentTo(string tempFile)
        {
            Log(LogLevel.Verbose, "Saving incoming mail attachment to '{0}'", tempFile);
        }

        public void SavedIncomingMailAttachmentTo(string tempFile)
        {
            Log(LogLevel.Information, "Saved incoming mail attachment to '{0}'", tempFile);
        }

        public void ConnectingToTfs(Uri tfsCollectionUri)
        {
            Log(LogLevel.Verbose, "Connecting to TFS at '{0}'", tfsCollectionUri.AbsoluteUri);
        }

        public void ConnectedToTfs(Uri tfsCollectionUri)
        {
            Log(LogLevel.Information, "Connected to TFS at '{0}'", tfsCollectionUri.AbsoluteUri);
        }

        public void TfsProjectNotFound(string projectName)
        {
            Log(LogLevel.Error, "TFS project '{0}' does not exist", projectName);
        }

        public void TfsWorkItemTypeNotFound(string projectName, string workItemTypeName)
        {
            Log(LogLevel.Error, "TFS work item '{1}' does not exist in project '{0}'", projectName, workItemTypeName);
        }

        public void AWorkItemAlreadyExistsFor(int existingId, MailMessage mail)
        {
            Log(LogLevel.Warning, "TFS work item for mail '{0}' already exist as #{0}", mail.Subject, existingId);
        }

        public void InvalidWorkItem(Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem workItem)
        {
            var buf = new StringBuilder();
            foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Field f in workItem.Fields)
            {
                if (!f.IsValid)
                {
                    buf.AppendLine();
                    buf.AppendFormat("File {0} [{1}] has invalid value '{2}'",
                        f.Name, f.ReferenceName, f.Value);
                }
            }
            Log(LogLevel.Error, "Work item has validation errors:{0}", buf.ToString());
        }

        public void SavingWorkItem(Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem workItem)
        {
            Log(LogLevel.Verbose, "Saving work item in TFS");
        }

        public void WorkItemSaved(Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem workItem)
        {
            Log(LogLevel.Information, "Saved work item #{0}", workItem.Id);
        }

        public void ComposingConfirmationMessage()
        {
            Log(LogLevel.Verbose, "Composing confirmation message");
        }

        public void SendingConfirmationMessageTo(string fromAddress)
        {
            Log(LogLevel.Verbose, "Sending confirmation message to '{0}'", fromAddress);
        }

        public void ConfirmationMessageSentTo(string fromAddress)
        {
            Log(LogLevel.Information, "Confirmation message sent to '{0}'", fromAddress);
        }

        public void FailToSendConfirmationMessage(System.Net.Mail.MailMessage message, Exception e)
        {
            Log(LogLevel.Critical, "Failed to send confirmation message to '{0}'\nException: {1}", message.Subject, e.Message);
        }

        public void UnexpectedError(Exception e)
        {
            Log(LogLevel.Critical, "Unexpected error {0}\nStack trace: {1}", e.Message, e.StackTrace);
        }

        public void RemovingTemporaryFolder(string tempFolder)
        {
            Log(LogLevel.Verbose, "Removing temporary folder '{0}'", tempFolder);
        }
    }
}
