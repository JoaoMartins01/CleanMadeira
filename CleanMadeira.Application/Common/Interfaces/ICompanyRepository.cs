using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Common.Interfaces
{
    public interface ICompanyRepository
    : IRepository<Company>
    {
        Task<List<Company>> GetCleaningCompaniesAsync();
        Task<Company?> GetByIdWithMembersAsync(Guid id);
        Task AddInvitationAsync(CompanyInvitation invitation);
        Task<CompanyInvitation?> GetInvitationByTokenAsync(Guid token);
        Task<bool> HasPendingInvitationAsync(Guid companyId, string email);
        Task UpdateInvitationAsync(CompanyInvitation invitation);

    }
}
