using ProgressControl.DAL.Entities;
using System;
using System.Threading.Tasks;

namespace ProgressControl.DAL.Interfaces
{
    public interface IUnitOfWork: IRepository<Element>, IRepository<Specification>, IRepository<Smt_box>
    {
        void Save();
    }
}
