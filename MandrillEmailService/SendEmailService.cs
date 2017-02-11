using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace MandrillEmailService
{
    public class EmailService
    {
        private const string mandrillUrl = "https://mandrillapp.com/api/1.0/messages/";
        private const string sendTemplateMethod = "send-template.json?";
        private const string sendMethod = "send.json?";
        private const string MLEmailTemplates = "MLEmail";
        private const string myFirstTemplate = "MyFirstTemplate";
        private const string associationRequest = "AssociationRequest";
        private const string arrival = "Arrival";
        //Akshat Mandrill Account
        private const string apikey = "20jDgCIitBZJ290h6tHOdA";
        //Suraj Mandrill Account
        //private const string apikey = "e7b1cef5-ba65-4731-9e05-a9722345c9bb";

        private const string apiKeyHighPriority = "xAiAlbx51Ng59elwXCrIqw";
        private const string sentStatus = "sent";
        private const int emailMinLength = 8;
        private const string logoImageTag = "<img src='{0}' alt='logo'></img>";
        private const string headerLogoPlaceHolder = "header_logo";
        private const string dearHeaderPlaceHolder = "DearHeader";
        private const string shortDescriptionPlaceHolder = "short_description";
        private const string longDescriptionPlaceHolder = "long_description";
        private const string footerPlaceHolder = "footer";
        private const string niueLogoPlaceHolder = "niue_logo";
        private const string niueFlagPlaceHolder = "niue_flag";
        private const string userNamePlaceHolder = "user_name";
        private const string emailContentPlaceHolder = "email_content";
        private const string mailAddressPlaceHolder = "mail_address";
        private const string textJsonContent = "text/json";
        private const string postMethod = "POST";
        private const string header_kioskName="header_MLName";


        public static void SendUserRegisterEmail(string fromEmail, string fromName, string subject, string recipientsEmail, string logoPath, string dearHeader,string longDescription,string shortDescription,string footer)
        {
            if (recipientsEmail != null)
            {
                List<Recipient> recipients = new List<Recipient>();
                if (recipientsEmail.Length >= emailMinLength)
                {
                    Recipient rec = new Recipient();
                    rec.email = recipientsEmail;
                    recipients.Add(rec);
                }

                if (recipients.Count == 0)
                {
                    return;
                }

                List<TemplateContent> templateContent = FillTemplateContent(logoPath, dearHeader, longDescription, shortDescription, footer);
                List<TemplateContent> templateContent1 = new List<TemplateContent>();
                string templateName = MLEmailTemplates;

                object message = new
                {
                    subject = subject,
                    from_email = fromEmail,
                    from_name = fromName,
                    to = recipients
                };

                object attachments = new
                {
                    type = "text/plain",
                    name = "D:\tttt.txt",
                    content = "tet"
                };

                object parameters = new
                {
                    key = apikey,
                    template_name = templateName,
                    template_content = templateContent,
                    message,
                    attachments
                };

                SendMail(parameters, fromEmail, sendTemplateMethod);
            }
        }

        public static void SendArrivalTemplateEmail(string fromEmail, string fromName, string subject, string recipientsEmail, string niuelogoPath, string niueflagPath, string userName, string emailcontent, string emailAddress)
        {
            if (recipientsEmail != null)
            {
                List<Recipient> recipients = new List<Recipient>();
                if (recipientsEmail.Length >= emailMinLength)
                {
                    Recipient rec = new Recipient();
                    rec.email = recipientsEmail;
                    recipients.Add(rec);
                }

                if (recipients.Count == 0)
                {
                    return;
                }

                List<TemplateContent> templateContent = FillArrivalTemplateContent(niuelogoPath, niueflagPath, userName, emailcontent, emailAddress);
                List<TemplateContent> templateContent1 = new List<TemplateContent>();
                string templateName = arrival;

                object message = new
                {
                    subject = subject,
                    from_email = fromEmail,
                    from_name = fromName,
                    to = recipients
                };

                object attachments = new
                {
                    type = "text/plain",
                    name = "D:\tttt.txt",
                    content = "tet"
                };

                object parameters = new
                {
                    key = apikey,
                    template_name = templateName,
                    template_content = templateContent,
                    message,
                    attachments
                };

                SendMail(parameters, fromEmail, sendTemplateMethod);
            }
        }
        public static void SendTemplateEmailWithAttachment(string fromEmail, string fromName, string subject, string[] recipientsEmails, string logoPath, string dearHeader,
            string longDescription,string shortDescription, string footer,string pdfPath,string pdfName)
        {
            if (recipientsEmails != null)
            {
                List<Recipient> recipients = new List<Recipient>();
                foreach (string recipientEmail in recipientsEmails)
                {
                    if (recipientEmail.Length >= emailMinLength)
                    {
                        Recipient rec = new Recipient();
                        rec.email = recipientEmail;
                        recipients.Add(rec);
                    }
                }

                if (recipients.Count == 0)
                {
                    return;
                }

                List<TemplateContent> templateContent = FillTemplateContent(logoPath, dearHeader, longDescription, shortDescription, footer);
                List<TemplateContent> templateContent1 = new List<TemplateContent>();
                string templateName = MLEmailTemplates;

                var pdfBytes = File.ReadAllBytes(pdfPath);
                //object attachment =;

                var attachments = new[]
                               {
                                    new
                                         {
                                             name = pdfName,
                                             type = "application/pdf",
                                             content = Convert.ToBase64String(pdfBytes)
                                         }
                               };

                object message = new
                {
                    subject = subject,
                    from_email = fromEmail,
                    from_name = fromName,
                    to = recipients,
                    attachments
                };
                
                object parameters = new
                {
                    key = apikey,
                    template_name = templateName,
                    template_content = templateContent,
                    message
                };

                SendMail(parameters, fromEmail, sendTemplateMethod);
            }
        }

        public static void SendEmailWithAttachment(string fromEmail, string fromName, string subject, List<string> recipientsEmail, string emailcontent, string pdfPath, string pdfName)
        {
            if (recipientsEmail != null)
            {
                List<Recipient> recipients = new List<Recipient>();
                foreach (string recipientEmail in recipientsEmail)
                {
                    if (recipientEmail != null && recipientEmail != "")
                    {
                        if (recipientEmail.Length >= emailMinLength)
                        {
                            Recipient rec = new Recipient();
                            rec.email = recipientEmail;
                            recipients.Add(rec);
                        }
                    }
                }

                if (recipients.Count == 0)
                {
                    return;
                }
                
                var pdfBytes = File.ReadAllBytes(pdfPath);

                var attachments = new[]
                               {
                                    new
                                         {
                                             name = pdfName,
                                             type = "application/pdf",
                                             content = Convert.ToBase64String(pdfBytes)
                                         }
                               };

                object message = new
                {
                    subject = subject,
                    from_email = fromEmail,
                    from_name = fromName,
                    to = recipients,
                    html = emailcontent,
                    attachments
                };

                object parameters = new
                {
                    key = apikey,
                    message
                };

                SendMail(parameters, fromEmail, sendMethod);
            }
        }
        

        public static void SendTemplateEmailsToSmallGroup(string fromEmail, string fromName, string subject, string[] recipientsEmails,
            string logoPath, string dearHeader, string longDescription, string shortDescription, string footer)
        {
            if (recipientsEmails != null)
            {
                List<Recipient> recipients = new List<Recipient>();
                foreach (string recipientEmail in recipientsEmails)
                {
                    if (recipientEmail.Length >= emailMinLength)
                    {
                        Recipient rec = new Recipient();
                        rec.email = recipientEmail;
                        recipients.Add(rec);
                    }
                }

                if (recipientsEmails.Length == 0)
                {
                    return;
                }

                List<TemplateContent> templateContent = FillTemplateContent(logoPath, dearHeader, longDescription, shortDescription, footer);

                string templateName = MLEmailTemplates;

                object message = new
                {
                    subject = subject,
                    from_email = fromEmail,
                    from_name = fromName,
                    to = recipientsEmails
                };

                object parameters = new
                {
                    key = apikey,
                    template_name = templateName,
                    template_content = templateContent,
                    message
                };

                SendMail(parameters, fromEmail, sendTemplateMethod);
            }
        }

        private static List<TemplateContent> FillArrivalTemplateContent(string niuelogoPath,string niueflagPath, string userName, string emailcontent, string emailAddress)
        {
            List<TemplateContent> templateContent = new List<TemplateContent>();

            if (!string.IsNullOrEmpty(niuelogoPath))
            {
                TemplateContent niueLogoContent = new TemplateContent();
                niueLogoContent.name = niueLogoPlaceHolder;
                niueLogoContent.content = String.Format(logoImageTag, niuelogoPath);
                templateContent.Add(niueLogoContent);
            }

            if (!string.IsNullOrEmpty(niueflagPath))
            {
                TemplateContent niueFlagContent = new TemplateContent();
                niueFlagContent.name = niueFlagPlaceHolder;
                niueFlagContent.content = niueflagPath;
                templateContent.Add(niueFlagContent);
            }

            if (!string.IsNullOrEmpty(userName))
            {
                TemplateContent userNameContent = new TemplateContent();
                userNameContent.name = userNamePlaceHolder;
                userNameContent.content = userName;
                templateContent.Add(userNameContent);
            }

            if (!string.IsNullOrEmpty(emailcontent))
            {
                TemplateContent emailContentContent = new TemplateContent();
                emailContentContent.name = emailContentPlaceHolder;
                emailContentContent.content = emailcontent;
                templateContent.Add(emailContentContent);
            }

            if (!string.IsNullOrEmpty(emailAddress))
            {
                TemplateContent emailAddressContent = new TemplateContent();
                emailAddressContent.name = mailAddressPlaceHolder;
                emailAddressContent.content = emailAddress;
                templateContent.Add(emailAddressContent);
            }
            return templateContent;
        }

        private static List<TemplateContent> FillTemplateContent(string logoPath, string dearHeader, string longDescription, string shortDescription, string footer)
        {
            List<TemplateContent> templateContent = new List<TemplateContent>();

            if (!string.IsNullOrEmpty(logoPath))
            {
                TemplateContent headerLogoContent = new TemplateContent();
                headerLogoContent.name = headerLogoPlaceHolder;
                headerLogoContent.content = String.Format(logoImageTag, logoPath);
                templateContent.Add(headerLogoContent);
            }

            if (!string.IsNullOrEmpty(dearHeader))
            {
                TemplateContent dearHeaderContent = new TemplateContent();
                dearHeaderContent.name = dearHeaderPlaceHolder;
                dearHeaderContent.content = dearHeader;
                templateContent.Add(dearHeaderContent);
            }

            if (!string.IsNullOrEmpty(longDescription))
            {
                TemplateContent longDescriptionContent = new TemplateContent();
                longDescriptionContent.name = longDescriptionPlaceHolder;
                longDescriptionContent.content = longDescription;
                templateContent.Add(longDescriptionContent);
            }

            if (!string.IsNullOrEmpty(shortDescription))
            {
                TemplateContent shortDescriptionContent = new TemplateContent();
                shortDescriptionContent.name = shortDescriptionPlaceHolder;
                shortDescriptionContent.content = shortDescription;
                templateContent.Add(shortDescriptionContent);
            }

            if (!string.IsNullOrEmpty(footer))
            {
                TemplateContent footerContent = new TemplateContent();
                footerContent.name = footerPlaceHolder;
                footerContent.content = footer;
                templateContent.Add(footerContent);
            }
            return templateContent;
        }

        public static void SendFormattedEmail(string fromEmail, string fromName, string subject, List<string> recipientsEmail,
            string emailContent)
        {
            List<Recipient> recipients = new List<Recipient>();
            foreach (string recipientEmail in recipientsEmail)
            {
                if (recipientEmail != null && recipientEmail != "")
                {
                    if (recipientEmail.Length >= emailMinLength)
                    {
                        Recipient rec = new Recipient();
                        rec.email = recipientEmail;
                        recipients.Add(rec);
                    }
                }
            }

            if (recipients.Count == 0)
            {
                return;
            }

            object message = new
            {
                subject = subject,
                from_email = fromEmail,
                from_name = fromName,
                to = recipients,
                html = emailContent
            };

            object parameters = new
            {
                key = apikey,
                message
            };
            SendMail(parameters, fromEmail, sendMethod);
        }

        private static void SendMail(object parameters, string fromEmail, string methodName)
        {
            StreamReader streamReader = null;
            StreamWriter streamWriter = null;
            try
            {
                var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string JsonString = jsonSerializer.Serialize(parameters);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Concat(mandrillUrl, methodName));
                httpWebRequest.ContentType = textJsonContent;
                httpWebRequest.Method = postMethod;
                var result = string.Empty;

                streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                streamWriter.Write(JsonString);
                streamWriter.Flush();
                streamWriter.Close();
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                streamReader = new StreamReader(httpResponse.GetResponseStream());
                result = streamReader.ReadToEnd();
                streamReader.Close();

                List<RecipientReturn> recipientReturns = new List<RecipientReturn>();
                object deserializedresult = jsonSerializer.Deserialize(result, typeof(List<RecipientReturn>));
                recipientReturns = (List<RecipientReturn>)deserializedresult;
                foreach (RecipientReturn recipientReturn in recipientReturns)
                {
                    if (string.Compare(recipientReturn.status, sentStatus, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        //EventLogUtility.WriteEntry("Cannot send email to " + recipientReturn.email + " from " + fromEmail
                        //    + ". Sent status: " + recipientReturn.status, EventMessageType.Warning);
                    }
                }
            }
            catch (Exception)
            {
                //EventLogUtility.WriteEntry("Error sending emails from " + fromEmail + Environment.NewLine + e.ToString(),
                //    EventMessageType.Error);
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Dispose();
                }
                if (streamWriter != null)
                {
                    streamWriter.Dispose();
                }
            }

        }
        public static void SendIndividualEmail(string fromEmail, string fromName, string subject, string recipientsEmail,
            string emailContent)
        {
            List<Recipient> recipients = new List<Recipient>();
            if (recipientsEmail.Length >= emailMinLength)
                {
                    Recipient rec = new Recipient();
                    rec.email = recipientsEmail;
                    recipients.Add(rec);
                }
          

            if (recipients.Count == 0)
            {
                return;
            }

            object message = new
            {
                subject = subject,
                from_email = fromEmail,
                from_name = fromName,
                to = recipients,
                html = emailContent
            };

            object parameters = new
            {
                key = apikey,
                message
            };
            SendMail(parameters, fromEmail, sendMethod);

        }
    }
}