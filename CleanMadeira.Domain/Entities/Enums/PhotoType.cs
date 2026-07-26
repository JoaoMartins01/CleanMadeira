using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CleanMadeira.Domain.Entities.Enums
{
    public enum PhotoType
    {
        Antes = 1,
        Depois = 2
    }
}
