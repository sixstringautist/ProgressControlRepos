using ProgressControl.DAL.Entities;
using ProgressControl.DAL.Interfaces;
using ProgressControl.DAL.EF;
using System;
using System.Threading.Tasks;
namespace ProgressControl.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        RsContext db;

        IRepository<Element> elementRepos;
        IRepository<Specification> specificationRepos;


        
        public IRepository<Element> Elements => elementRepos;
        
        public IRepository<Specification> Specifications => specificationRepos;

        public UnitOfWork(RsContext db)
        {
            this.db = db;
            elementRepos = new ElementsRepo(db);
            specificationRepos = new SpecificationsRepo(db);
        }


        private bool disposed = false;
        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Elements.Dispose();
                    Specifications.Dispose();
                }
                this.disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public async Task<int> SaveAsync()
        {
            return await db.SaveChangesAsync();
        }
    }
}
