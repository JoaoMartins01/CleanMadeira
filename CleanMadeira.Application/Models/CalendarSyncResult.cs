using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMadeira.Application.Models
{
    public class CalendarSyncResult
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public int ReservationsCreated { get; set; }

        public int ReservationsUpdated { get; set; }

        public int CleaningTasksCreated { get; set; }
    }
}
