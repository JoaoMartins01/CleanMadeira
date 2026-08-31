namespace CleanMadeira.Web.ViewModels.Company
{
    public class AcceptInvitationVM
    {
        public Guid Token { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }
    }
}
