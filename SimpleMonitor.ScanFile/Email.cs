using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMonitor.ScanFile
{
    public class Email
    {
        public static void SendEmail(string result)
        {
            string emailStatus = string.Empty;

            string from = ApplicationSetting.SmtpUserName;

            //Replace this with the Email Address to whom you want to send the mail
            string to = ApplicationSetting.SmtpTo;

            string fromName = "Poor Man Monitor";
            string userName = ApplicationSetting.SmtpUserName;
            string password = ApplicationSetting.SmtpPassword;

            Attachment attachment = new Attachment(result);


            //if you want to send attachment, include the file location
            // Attachment attachment = new Attachment("file location");

            SmtpClient client = new SmtpClient();
            client.EnableSsl = true; //Gmail works on Server Secured Layer

            client.Credentials = new System.Net.NetworkCredential(userName, password);
            client.Port = int.Parse(ApplicationSetting.SmtpPort);
            client.Host = ApplicationSetting.SmtpHost;

            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(from, fromName);
                    mail.Sender = mail.From;

                    if (to.Contains(";"))
                    {
                        string[] _EmailsTO = to.Split(";".ToCharArray());
                        for (int i = 0; i < _EmailsTO.Length; i++)
                        {
                            mail.To.Add(new MailAddress(_EmailsTO[i]));
                        }
                    }
                    else
                    {
                        if (!to.Equals(string.Empty))
                        {
                            mail.To.Add(new MailAddress(to));
                        }
                    }


                    string message = "this is a test message";

                    mail.Subject = ApplicationSetting.SmtpSubject;

                    mail.Body = "<div style=\"font: 11px verdana, arial\">";
                    mail.Body += message.Replace("\n", "<br />") + "<br /><br />";
                    mail.Body += "<hr /><br />";
                    mail.Body += "<h3>Sender information</h3>";
                    mail.Body += "<div style=\"font-size:10px;line-height:16px\">";
                    mail.Body += "<strong>Name:</strong> " + fromName + "<br />";
                    mail.Body += "<strong>E-mail:</strong> " + from + "<br />";
                    mail.IsBodyHtml = true;

                    //attachment
                    if (attachment != null)
                    {
                        mail.Attachments.Add(attachment);
                    }

                    client.Send(mail);
                    emailStatus = "success";
                }
            }

            catch (Exception ex)
            {
                emailStatus = ex.Message;
            } // end try 
        }
    }
}
