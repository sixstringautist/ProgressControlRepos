using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressControl.DAL.Interfaces
{
    public interface IRepository<T> : IDisposable where T: class
    {

        IEnumerable<T> GetAll();
        T Get(Func<T,bool> predicate);
        IEnumerable<T> Filter(Func<T, bool> predicate);
        bool Create(T item);
        bool Update(T item);
        bool Delete(T Item);
    }
}
