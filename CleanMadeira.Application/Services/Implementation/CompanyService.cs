using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<IEnumerable<Company>> GetAllAsync()
    {
        return await _companyRepository.GetAllAsync();
    }
    public async Task<List<Company>> GetCleaningCompaniesAsync()
    {
        return await _companyRepository
            .GetCleaningCompaniesAsync();
    }

    public async Task<Company?> GetByIdAsync(Guid? id)
    {
        return await _companyRepository.GetByIdAsync(id);
    }

    public async Task<Company?> GetByIdWithMembersAsync(Guid id)
    {
        return await _companyRepository
            .GetByIdWithMembersAsync(id);
    }

    public async Task AddInvitationAsync(
    CompanyInvitation invitation)
    {
        await _companyRepository
            .AddInvitationAsync(invitation);
    }

    public async Task<bool> HasPendingInvitationAsync(
        Guid companyId,
        string email)
    {
        return await _companyRepository
            .HasPendingInvitationAsync(
                companyId,
                email);
    }

    public async Task<CompanyInvitation?>
        GetInvitationByTokenAsync(Guid token)
    {
        return await _companyRepository
            .GetInvitationByTokenAsync(token);
    }

    public async Task UpdateInvitationAsync(
    CompanyInvitation invitation)
    {
        await _companyRepository
            .UpdateInvitationAsync(invitation);
    }

    public async Task CreateAsync(Company company)
    {
        company.Id = Guid.NewGuid();
        company.Active = true;
        company.CreatedAt = DateTime.UtcNow;

        await _companyRepository.AddAsync(company);
        await _companyRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(Company company)
    {
        await _companyRepository.UpdateAsync(company);
        await _companyRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var company = await _companyRepository.GetByIdAsync(id);

        if (company == null)
            return;

        company.Active = false;

        await _companyRepository.UpdateAsync(company);
        await _companyRepository.SaveChangesAsync();
    }
}