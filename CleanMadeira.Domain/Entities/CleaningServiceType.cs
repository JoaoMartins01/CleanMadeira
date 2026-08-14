using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMadeira.Domain.Entities
{
    public class CleaningServiceType
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Guid OwnerId { get; set; }

        public bool Active { get; set; } = true;
    }
}
