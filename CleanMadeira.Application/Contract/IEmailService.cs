using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Contract
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendConfirmationEmailAsync(
        ApplicationUser user,
        string confirmationLink);

        Task SendWelcomeEmailAsync(
            ApplicationUser user, string loginLink);

        Task SendResetPasswordEmailAsync(
            ApplicationUser user,
            string resetLink);

        Task SendCleaningAssignedEmailAsync(
            ApplicationUser cleaner,
            CleaningTask task,
            Property property,
            string taskDetailsLink);
    }
}
