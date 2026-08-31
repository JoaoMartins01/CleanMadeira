using CleanMadeira.Domain.Enums;

namespace CleanMadeira.Domain.Entities
{
    public class Company
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string NIF { get; set; }

        public Guid CreatedByUserId { get; set; }

        public ICollection<ApplicationUser> Users { get; set; }
        = new List<ApplicationUser>();

        public ICollection<Property> Properties { get; set; }
            = new List<Property>();

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string Phone { get; set;  }
        public CompanyType Type { get; set; }

        public CompanyStatus Status { get; set; }
        = CompanyStatus.Pendente;

        public bool Active { get; set; }

        public DateTime CreatedAt { get; set; }


    }
}
