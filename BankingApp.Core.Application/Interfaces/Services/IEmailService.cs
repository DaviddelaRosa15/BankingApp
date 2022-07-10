using BankingApp.Core.Application.DTOs.Email;
using BankingApp.Core.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.Interfaces.Services
{
    public interface IEmailService
    {
        public MailSettings _mailSettings { get; }
        Task SendAsync(EmailRequest request);
    }
}
