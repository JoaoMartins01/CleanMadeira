using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMadeira.Domain.Entities
{
    public class Company
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Nif { get; set; }

        public ICollection<ApplicationUser> Users { get; set; }
        = new List<ApplicationUser>();

        public ICollection<Property> Property { get; set; }
            = new List<Property>();

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Adress { get; set; }

        public bool Active { get; set; }

        public DateTime CreatedAt { get; set; }


    }
}
