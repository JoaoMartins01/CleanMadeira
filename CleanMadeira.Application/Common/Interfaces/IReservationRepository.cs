using CleanMadeira.Application.Services.Implementation;
using CleanMadeira.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMadeira.Application.Common.Interfaces
{
    public interface IReservationRepository
    {
        Task<Reservation?> GetByExternalUidAsync(
            Guid calendarIntegrationId,
            string externalUid);

        Task AddAsync(Reservation reservation);

        Task SaveChangesAsync();
    }
}
