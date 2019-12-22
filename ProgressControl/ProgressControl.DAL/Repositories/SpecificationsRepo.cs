using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgressControl.DAL.Interfaces;
using ProgressControl.DAL.Entities;
using ProgressControl.DAL.EF;

namespace ProgressControl.DAL.Repositories
{
    public class SpecificationsRepo : IRepository<Specification>
    {
        RsContext db;

        public SpecificationsRepo(RsContext db)
        {
            this.db = db;
        }

        public void Create(Specification item)
        {
            db.Specifications.Add(item);
        }

        public void Delete(int id)
        {
            var sp = db.Specifications.Find(id);
            if (sp != null)
            {
                db.Specifications.Remove(sp);
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public IEnumerable<Specification> Filter(Func<Specification, bool> predicate)
        {
            return db.Specifications.Where(predicate).ToList();
        }

        public Specification Get(int id)
        {
            return db.Specifications.Find(id);
        }

        public IEnumerable<Specification> GetAll()
        {
            return db.Specifications.ToList();
        }

        public void Update(Specification item)
        {
            db.Entry(item).State = System.Data.Entity.EntityState.Modified;
        }
    }
}
