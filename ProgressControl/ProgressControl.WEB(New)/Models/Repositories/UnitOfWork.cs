using ProgressControl.DAL.Entities;
using ProgressControl.DAL.Interfaces;
using ProgressControl.DAL.EF;
using System;
using System.Runtime.CompilerServices;
using Hangfire.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
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

        private IEnumerable<TResult> GetAll<TResult>(DbSet<TResult> set)
            where TResult : class
        {
            return set.ToList();
        }

        public IEnumerable<TResult> GetAll<TResult>()
           where TResult  : DBObject<int>
        {
            return (this as IRepository<TResult>).GetAll();
        }

        IEnumerable<Element> IRepository<Element>.GetAll()
        {
            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                return db.Elements.ToList();
            }
            else return null;
        }


        private TResult Get<TResult>(DbSet<TResult> set, Func<TResult, bool> predicate)
            where TResult : DBObject<int>
        {
            CheckNull(set);
            CheckNull(predicate);
            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                return set.FirstOrDefault(predicate);
            }
            else return null;

        }
        public Element Get(Func<Element, bool> predicate)
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
        private void CheckNull<T>(params T[] par)
            where T : class
        {
            foreach (var el in par)
            {
                if(el == null)
                    throw new ArgumentNullException($"{nameof(el)} cannot be null") { };
            }
        }
        private IEnumerable<TResult> Filter<TResult>(DbSet<TResult> set, Func<TResult, bool> predicate)
            where TResult : class
        {
            CheckNull(predicate);
            CheckNull(set);
            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                return set.Where(predicate);
            }
            else return new List<TResult>();
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

        private bool Create<TCreate>(DbSet<TCreate> set, TCreate item)
            where TCreate : DBObject<int>
        {
            CheckNull(set);
            CheckNull(item);
            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                if (db.Elements.Find(item.Code) == null)
                {
                    set.Add(item);
                    return true;
                }
                else throw new Exception($"Element with code{item.Code} alredy exist");
            }
            return false;
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


        private bool Delete<TDelete>(DbSet<TDelete> set, TDelete item)
            where TDelete : DBObject<int>
        {
            CheckNull(set);
            CheckNull(item);
            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                if (set.Find(item.Code) != null)
                {
                    set.Remove(item);
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

        IEnumerable<RsTask> IRepository<RsTask>.GetAll()
        {
            return db.GlobalTasks.ToList();
        }

        public RsTask Get(Func<RsTask, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("Predicate cannot be null");
            return db.GlobalTasks.FirstOrDefault(predicate);
        }

        public IEnumerable<RsTask> Filter(Func<RsTask, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("Predicate cannot be null");
            return db.GlobalTasks.Where(predicate);
        }

        public bool Create(RsTask item)
        {
            return Create(db.GlobalTasks, item);
        }

        public bool Update(RsTask item)
        {
            CheckNull(item);
            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                var tmp = db.GlobalTasks.FirstOrDefault(x => x.Code == item.Code);
                if (tmp != null)
                {
                    tmp.Code = item.Code;
                    tmp.NavProp = item.NavProp;
                    tmp.Subtasks = item.Subtasks;
                    return true;
                }
            }
                return false;
        }

        public bool Delete(RsTask Item)
        {
            return Delete(db.GlobalTasks, Item);
        }

        IEnumerable<RsArea> IRepository<RsArea>.GetAll()
        {
            return GetAll(db.RsAreas);
        }

        public RsArea Get(Func<RsArea, bool> predicate)
        {
            return Get(db.RsAreas, predicate);
        }

        public IEnumerable<RsArea> Filter(Func<RsArea, bool> predicate)
        {
            return Filter(db.RsAreas, predicate);
        }

        public bool Create(RsArea item)
        {
            return Create(db.RsAreas, item);
        }

        public bool Update(RsArea item)
        {
            CheckNull(item);
            d = hangfireStorage.GetJobData("DBF_Connector.BackgroundTask");
            if (d.State != "Processing")
            {
                var tmp = db.RsAreas.FirstOrDefault(x => x.Code == item.Code);
                if (tmp != null)
                {
                    tmp.Code = item.Code;
                    tmp.Name = item.Name;
                    return true;
                }
            }
            return false;
        }

        public bool Delete(RsArea Item)
        {
            return Delete(db.RsAreas, Item);
        }

        IEnumerable<AreaTask> IRepository<AreaTask>.GetAll()
        {
            return GetAll(db.AreaTasks);
        }

        public AreaTask Get(Func<AreaTask, bool> predicate)
        {
            return Get(db.AreaTasks, predicate);
        }

        public IEnumerable<AreaTask> Filter(Func<AreaTask, bool> predicate)
        {
            return Filter(db.AreaTasks, predicate);
        }

        public bool Create(AreaTask item)
        {
            return Create(db.AreaTasks, item);
        }

        public bool Update(AreaTask item)
        {
            throw new NotImplementedException();
        }

        public bool Delete(AreaTask Item)
        {
            return Delete(db.AreaTasks, Item);
        }

        IEnumerable<Subtask> IRepository<Subtask>.GetAll()
        {
            return GetAll(db.SubTasks);
        }

        public Subtask Get(Func<Subtask, bool> predicate)
        {
            return Get(db.SubTasks, predicate);
        }

        public IEnumerable<Subtask> Filter(Func<Subtask, bool> predicate)
        {
            return Filter(db.SubTasks, predicate);
        }

        public bool Create(Subtask item)
        {
            return Create(db.SubTasks, item);
        }

        public bool Update(Subtask item)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Subtask Item)
        {
            return Delete(db.SubTasks, Item);
        }
    }
}
