using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Infrastructure.Data;
using CleanMadeira.Infrastructure.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteLagoon.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IPropertyRepository Property { get; private set;}
        public ICleaningTaskRepository CleaningTask { get; private set; }
        public IUtilizadorRepositorio Utilizador { get; private set; }
        public IApplicationUserRepository User { get; private set; }
        public ICompanyRepository Company { get; private set;}
        public IInventoryRepository Inventory { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Property = new PropertyRepository(_db);
            Company = new CompanyRepository(_db);
            CleaningTask = new CleaningTaskRepository(_db);
            //Utilizador = new UtilizadorRepositorio(_db);
            Inventory = new InventoryRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
