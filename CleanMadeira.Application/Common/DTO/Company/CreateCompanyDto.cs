using CleanMadeira.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMadeira.Application.Common.DTO.Company
{
    public class CreateCompanyDto
    {
        public string Name { get; set; } = string.Empty;
        public CompanyType Type { get; set; }
    }
}
