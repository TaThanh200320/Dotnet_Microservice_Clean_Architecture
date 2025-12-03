using Contracts.Dtos.Requests;

namespace Application.Interfaces.Services.Mail;

public interface IMailService
{
    Task<bool> SendAsync(MailMessageData metaData);
    Task<bool> SendWithTemplateAsync(MailTemplateData metaData);
}
