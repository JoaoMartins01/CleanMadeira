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
    public enum CleaningStatus
    {
        Pendente = 1,
        Atribuido = 2,
        EmProgresso = 3,
        Completo = 4,
        Cancelado = 5
    }
}
