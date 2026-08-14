using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Domain.Entities;

public interface ICompanyService : IService<Company>
{
}
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

    public async Task<Company?> GetByIdAsync(Guid? id)
    {
        return await _companyRepository.GetByIdAsync(id);
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