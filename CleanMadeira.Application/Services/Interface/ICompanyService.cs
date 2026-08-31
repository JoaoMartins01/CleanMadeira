using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Services.Interface
{
    public interface ICompanyService : IService<Company>
    {
        Task<List<Company>> GetCleaningCompaniesAsync();
        Task<Company?> GetByIdWithMembersAsync(Guid id);

        Task AddInvitationAsync(
            CompanyInvitation invitation);

        Task<bool> HasPendingInvitationAsync(
            Guid companyId,
            string email);

        Task<CompanyInvitation?>
            GetInvitationByTokenAsync(Guid token);

        Task UpdateInvitationAsync(
            CompanyInvitation invitation);
    }
}
