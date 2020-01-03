using ProgressControl.DAL.Entities;
using ProgressControl.DAL.Interfaces;
using ProgressControl.DAL.EF;
using System;
using System.Threading.Tasks;
using Hangfire.Storage;
using Hangfire.States;
using System.Collections.Generic;
using ProgressControl.DAL.Entities;
using System.Collections.Generic;
using System.Linq;
namespace ProgressControl.WEB_New_.Model.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        RsContext db;

        IStorageConnection hangfireStorage;
        JobData d;

       

        public UnitOfWork(RsContext db, IStorageConnection hangfireStorage)
        {
            this.db = db;
            this.hangfireStorage = hangfireStorage;
        }



        private bool disposed = false;
        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                this.disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Save()
        {
            db.SaveChangesAsync();
        }

        public IEnumerable<Element> GetAll()
        {
            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                return db.Elements.ToList();
            }
            else return null;
        }

        public Element Get(Func<Element,bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("Predicate cannot be null");

            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                return db.Elements.FirstOrDefault(predicate);
            }
            else return null;
        }

        public IEnumerable<Element> Filter(Func<Element, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("Predicate cannot be null");

            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                return db.Elements.Where(predicate);
            }
            else return null;
        }

        public bool Create(Element item)
        {
            if (item == null)
                throw new ArgumentNullException("Item cannot be null");
            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                if (db.Elements.Find(item.Code) == null)
                {
                    db.Elements.Add(item);
                    return true;
                }
                else throw new Exception($"Element with code{item.Code} alredy exist");
            }
            return false;
        }

        public bool Update(Element item)
        {
            if (item == null)
                throw new ArgumentNullException("Item cannot be null");
            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                var tmp = db.Elements.Find(item.Code);
                if (tmp != null)
                {
                    tmp.Childrens = item.Childrens;
                    tmp.Parents = item.Parents;
                    tmp.Quantity = item.Quantity;
                    tmp.Un = item.Un;
                    tmp.Values = item.Values;
                    tmp.Collection = item.Collection;
                    tmp.Code = item.Code;
                    return true;
                }
            }
            return false;
        }

        public bool Delete(Element item)
        {
            if (item == null)
                throw new ArgumentNullException("Item cannot be null");

            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                if (db.Elements.Find(item.Code) != null)
                {
                    db.Elements.Remove(item);
                    return true;
                }
            }
            return false;
        }

        IEnumerable<Specification> IRepository<Specification>.GetAll()
        {
            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                return db.Specifications.ToList();
            }
            return null;
        }



            public IEnumerable<Specification> Filter(Func<Specification, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("Predicate cannot be null");
            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                return db.Specifications.Where(predicate);
            }
            return null;
        }

        public bool Create(Specification item)
        {
            if (item == null)
                throw new ArgumentNullException("Item cannot be null");
            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                if (db.Specifications.Find(item.Code) == null)
                {
                    db.Specifications.Add(item);
                }
                else throw new Exception($"Element with code{item.Code} alredy exist");

            }
            return false;
        }

        public bool Update(Specification item)
        {
            if (item == null)
                throw new ArgumentNullException("Item cannot be null");
            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                var tmp = db.Specifications.Find(item.Code);
                if (tmp != null)
                {
                    tmp.Code = item.Code;
                    tmp.Collection = item.Collection;
                    tmp.Date = item.Date;
                    tmp.Name = item.Name;
                    return true;
                }
            }
            return false;
        }

        IEnumerable<Smt_box> IRepository<Smt_box>.GetAll()
        {
            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                return db.SmtBoxes.ToList();
            }
            return null;
        }


            public IEnumerable<Smt_box> Filter(Func<Smt_box, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("Item cannot be null");
            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                return db.SmtBoxes.Where(predicate);
            }
            return null;
        }

        public bool Create(Smt_box item)
        {
            if (item == null)
                throw new ArgumentNullException("Item cannot be null");
            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                if (db.SmtBoxes.FirstOrDefault(x => x.Code == item.Code && x.ElementId == item.ElementId) == null)
                {
                    db.SmtBoxes.Add(item);
                    return true;
                }
                else throw new Exception($"Smnt_box with");
            }

            return false;
        }

        public bool Update(Smt_box item)
        {
            if (item == null)
                throw new ArgumentNullException("Item cannot be null");
            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                var tmp = db.SmtBoxes.FirstOrDefault(x => x.ElementId == item.ElementId && x.Code == item.Code);
                if (tmp != null)
                {
                    tmp.Quantity = item.Quantity;
                    tmp.CreationDate = item.CreationDate;
                    tmp.ElementId = item.ElementId;
                    tmp.Spent = item.Spent;
                    tmp.InFeeder = item.InFeeder;
                    return true;
                }
            }
            return false;
        }

        public Specification Get(Func<Specification, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("Predicate cannot be null");
            return db.Specifications.FirstOrDefault(predicate);
        }

        public Smt_box Get(Func<Smt_box, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("Predicate cannot be null");
            return db.SmtBoxes.FirstOrDefault(predicate);
        }

        public bool Delete(Specification Item)
        {
            if (Item == null)
                throw new ArgumentNullException("Item cannot be null");
            if (db.Specifications.Find(Item.Code) == null)
                return false;
            db.Specifications.Remove(Item);
            return true;

        }

        public bool Delete(Smt_box Item)
        {
            if (Item == null)
                throw new ArgumentNullException("Item cannot be null");
            if (db.SmtBoxes.FirstOrDefault(x=> x.Code == Item.Code && x.ElementId == Item.ElementId) == null)
                return false;
            db.SmtBoxes.Remove(Item);
            return true;
        }
    }
}
