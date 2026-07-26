using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Application.Services.Implementation;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMadeira.Infrastructure.Repository
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Reservation?> GetByExternalUidAsync(
            Guid calendarIntegrationId,
            string externalUid)
        {
            return await _context.Reservations
                .Include(x => x.CleaningTask)
                .FirstOrDefaultAsync(x =>
                    x.CalendarIntegrationId == calendarIntegrationId &&
                    x.ExternalUid == externalUid);
        }

        public async Task AddAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
