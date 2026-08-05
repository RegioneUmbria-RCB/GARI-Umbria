using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using InData.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.AspNetCore.StaticFiles;
using System.Net.Mime;
using AgronicaCoreModelsSTD.AgronicaChatGPT;
using Microsoft.Extensions.Configuration;
using AgronicaCoreDTOStd.SmartTractors_HubIoT;
using System.ComponentModel;

namespace AgronicaNetCore.Base.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly ISecurityLayerDAL _securityLayerDAL;
        private readonly IConfiguration _config;
        public EmailService(ISecurityLayerDAL securityLayerDAL, IConfiguration config)
        {
            _securityLayerDAL = securityLayerDAL;
            _config = config;
        }

        public async Task<bool> SendEmailAsync(AgronicaCoreParametriServer objParametriServer, EmailData emailData)
        {
            if (ServicePointManager.SecurityProtocol != SecurityProtocolType.Tls12)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }

            using var mail = new MailMessage();
            mail.From = new MailAddress(emailData.From);

            if (!string.IsNullOrEmpty(emailData.ToConcatenated))
            {
                mail.To.Add(emailData.ToConcatenated);
            }

            if (!string.IsNullOrEmpty(emailData.CcConcatenated))
            {
                mail.CC.Add(emailData.CcConcatenated);
            }

            if (!string.IsNullOrEmpty(emailData.CcnConcatenated))
            {
                mail.Bcc.Add(emailData.CcnConcatenated);
            }

            mail.Subject = emailData.Subject;
            mail.IsBodyHtml = emailData.IsBodyHtml;
            mail.Body = emailData.Text;

            var provider = new FileExtensionContentTypeProvider();
            foreach (var attachment in emailData.Attachments)
            {
                if (File.Exists(attachment))
                {
                    var attachmentInfo = new FileInfo(attachment);
                    string mediaType;

                    if (!provider.TryGetContentType(attachmentInfo.Name, out mediaType))
                    {
                        mediaType = "application/octet-stream"; // Default fallback
                    }
                    var contentType = new ContentType();
                    contentType.MediaType = mediaType;

                    mail.Attachments.Add(new Attachment(attachment, contentType));
                }
                else
                {
                    mail.Attachments.Add(new Attachment(attachment));
                }
            }

            try
            {
                var dt = await _securityLayerDAL.LeggiConfigurazioneSitiAsync("ClientSMTP", objParametriServer);
                var clientSMTP = (string)dt.Rows[0]["Valore"];

                if (string.IsNullOrEmpty(clientSMTP))
                {
                    throw new Exception("Client SMTP non configurato");
                }

                dt = await _securityLayerDAL.LeggiConfigurazioneSitiAsync("user_smtp", objParametriServer);
                var user = (string)dt.Rows[0]["Valore"];

                var password = await Utility.Security.ReadEncryptedFieldFromDb(_securityLayerDAL, _config, objParametriServer, "password_smtp");


                var smtpClient = new SmtpClient(clientSMTP);
                smtpClient.Credentials = new NetworkCredential(user, password);

                dt = await _securityLayerDAL.LeggiConfigurazioneSitiAsync("enablessl_smtp", objParametriServer);

                if (dt.Rows.Count > 0)
                {
                    if (bool.TryParse((string)dt.Rows[0]["Valore"], out bool enableSsl))
                    {
                        smtpClient.EnableSsl = enableSsl;
                    }
                }

                dt = await _securityLayerDAL.LeggiConfigurazioneSitiAsync("ClientSMTP_Porta", objParametriServer);

                if (dt.Rows.Count > 0)
                {
                    if (int.TryParse((string)dt.Rows[0]["Valore"], out int port))
                    {
                        smtpClient.Port = port;
                    }
                }

                await smtpClient.SendMailAsync(mail);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
    }
}
