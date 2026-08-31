using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMadeira.Domain.Entities
{
    public class CompanyInvitation
    {
        public Guid Id { get; set; }

        public Guid CompanyId { get; set; }

        public Company? Company { get; set; }

        public string Email { get; set; } = string.Empty;

        public Guid Token { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool Accepted { get; set; }
    }
}
