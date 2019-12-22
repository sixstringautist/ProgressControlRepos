﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressControl.DAL.Interfaces
{
    public interface IRepository<T> : IDisposable where T: class
    {

        IEnumerable<T> GetAll();
        T Get(int id);
        IEnumerable<T> Filter(Func<T, bool> predicate);
        void Create(T item);
        void Update(T item);
        void Delete(int id);
    }
}
