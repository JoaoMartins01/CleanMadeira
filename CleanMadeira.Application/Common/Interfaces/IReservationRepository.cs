using CleanMadeira.Domain.Entities;

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
