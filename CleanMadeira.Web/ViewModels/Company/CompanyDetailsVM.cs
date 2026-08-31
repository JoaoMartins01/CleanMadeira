namespace CleanMadeira.Web.ViewModels.Company
{
    public class CompanyDetailsVM
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string NIF { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public int PropertyCount { get; set; }

        public int MemberCount { get; set; }

        public List<CompanyMemberVM> Members { get; set; }
            = new();
    }
}
