using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMadeira.Domain.Entities.Enums
{
    public enum MaintenanceReportStatus
    {
        PendingReview = 1,
        ConvertedToMaintenance = 2,
        ResolvedWithoutMaintenance = 3,
        Rejected = 4
    }
}
