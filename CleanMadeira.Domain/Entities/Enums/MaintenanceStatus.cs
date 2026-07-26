using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMadeira.Domain.Entities.Enums
{
    public enum MaintenanceStatus
    {
        Pendente = 1,
        Aceite = 2,
        EmProgresso = 3,
        Completo = 4,
        Cancelado = 5,
        Rejeitada = 6
    }

}
