using CleanMadeira.Domain.Entities.Enums;

namespace CleanMadeira.Domain.Entities
{
    public class Utilizador
    {
        public Guid Id { get; set; }
        public Guid? CompanyId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumeber { get; set; }

        public string PasswordHash { get; set; }

        public UserRole Role { get; set; }

        public bool Active { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
