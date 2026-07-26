using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMadeira.Domain.Entities.Enums
{
    public enum UserRole
    {
        Dono = 1,
        Supervisor = 2,
        Limpador = 3,
        Admin = 4
    }
}
