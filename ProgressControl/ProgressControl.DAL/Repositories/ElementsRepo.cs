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
    public class ElementsRepo : IRepository<Element>
    {

        private RsContext db;

        public ElementsRepo(RsContext db)
        {
            this.db = db;
        }
        public void Create(Element item)
        {
            db.Elements.Add(item);
        }

        public void Delete(int id)
        {
            var el = db.Elements.Find(id);
            if (el != null)
            {
                db.Elements.Remove(el);
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }
       
        public IEnumerable<Element> Filter(Func<Element, bool> predicate)
        {
            return db.Elements.ToList();
        }

        public Element Get(int id)
        {
            return db.Elements.Find(id);
        }

        public IEnumerable<Element> GetAll()
        {
            return db.Elements.ToList();
        }

        public void Update(Element item)
        {
            db.Entry(item).State = System.Data.Entity.EntityState.Modified;
        }
    }
}
