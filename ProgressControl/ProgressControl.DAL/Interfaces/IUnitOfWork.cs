using ProgressControl.DAL.Entities;
using System;
using System.Threading.Tasks;

namespace ProgressControl.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Element> Elements { get;}
        IRepository<Specification> Specifications { get; }
        Task<int> SaveAsync();
    }
}
