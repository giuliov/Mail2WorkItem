using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.IO;
using SmartFormat;

namespace Mail2WorkItem.Core
{
    public class Engine
    {
        EngineConfiguration configuration;
        ILogEvents logger;

        public Engine(EngineConfiguration configuration, ILogEvents logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public void Run()
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), "Mail2WorkItem");
            Directory.CreateDirectory(tempFolder);

            try
            {
                var picker = new MailPicker(
                        this.logger,
                        configuration.Pop3.Hostname, configuration.Pop3.Port, configuration.Pop3.UseSsl,
                        configuration.Pop3.Username, configuration.Pop3.Password,
                        tempFolder);
                var messages = picker.ReadAll();

                if (messages.Count > 0)
                {
                    var tfs = new TfsFacade(
                            this.logger,
                            new Uri(configuration.TFS.CollectionUrl), configuration.TFS.ProjectName,
                            configuration.TFS.WorkItemType);
                    foreach (var mail in messages)
                    {
                        int workItemId = tfs.AddWorkItem(mail);
                        var model = PrepareConfirmationModel(tfs, mail, workItemId);
                        SendConfirmation(mail.From, model);
                    }//for
                }//if
            }
            catch (Exception e)
            {
                logger.UnexpectedError(e);
            }
            finally
            {
                logger.RemovingTemporaryFolder(tempFolder);
                Directory.Delete(tempFolder, true);
            }//try
        }

        private ConfirmationModel PrepareConfirmationModel(TfsFacade tfs, MailMessage mail, int workItemId)
        {
            var model = new ConfirmationModel()
            {
                WorkItemType = configuration.TFS.WorkItemType,
                WorkItemId = workItemId,
                WorkItemUrl = tfs.GetWorkItemUrl(workItemId),
                Requester = mail.From.DisplayName,
                OriginalSubject = mail.Subject,
            };
            return model;
        }

        private void SendConfirmation(OpenPop.Mime.Header.RfcMailAddress fromAddress, ConfirmationModel model)
        {
            logger.ComposingConfirmationMessage();

            var message = new System.Net.Mail.MailMessage();
            message.From = new MailAddress(configuration.Smtp.From, configuration.Confirmation.Sender);
            message.To.Add(new MailAddress(fromAddress.Address, fromAddress.DisplayName));
            message.Subject = Smart.Format(configuration.Confirmation.Title, model);
            message.IsBodyHtml = true;
            message.Body = Smart.Format(configuration.Confirmation.Body, model);

            var smtp = new SmtpClient(configuration.Smtp.Hostname, configuration.Smtp.Port);
            smtp.EnableSsl = configuration.Smtp.UseSsl;
            smtp.Credentials = new System.Net.NetworkCredential(configuration.Smtp.Username, configuration.Smtp.Password);

            try
            {
                logger.SendingConfirmationMessageTo(fromAddress.Address);
                smtp.Send(message);
                logger.ConfirmationMessageSentTo(fromAddress.Address);
            }
            catch (Exception e)
            {
                logger.FailToSendConfirmationMessage(message, e);
            }//try
        }
    }

    class ConfirmationModel
    {
        public string WorkItemType { get; set; }
        public int WorkItemId { get; set; }
        public string WorkItemUrl { get; set; }
        public string Requester { get; set; }
        public string OriginalSubject { get; set; }
    }
}
