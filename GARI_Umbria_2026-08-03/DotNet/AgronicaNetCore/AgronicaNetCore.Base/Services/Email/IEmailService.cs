using AgronicaNetCore.Base.Models;
using InData.Email;

namespace AgronicaNetCore.Base.Services.Email
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(AgronicaCoreParametriServer objParametriServer, EmailData emailData);
    }
}
