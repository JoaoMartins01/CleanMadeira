namespace CleanMadeira.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IPropertyRepository Property { get; }
        IUtilizadorRepositorio Utilizador { get; }
        IApplicationUserRepository User { get; }
        ICompanyRepository Company { get; }
        IInventoryRepository Inventory { get; }
        ICleaningTaskRepository CleaningTask { get; }
        void Save();
    }
}
