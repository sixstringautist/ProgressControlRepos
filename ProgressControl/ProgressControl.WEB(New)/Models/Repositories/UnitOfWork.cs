using Hangfire.Storage;
using PagedList;
using ProgressControl.DAL.EF;
using ProgressControl.DAL.Entities;
using ProgressControl.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
namespace ProgressControl.WEB_New_.Model.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {

        RsContext db;

        IStorageConnection hangfireStorage;
        RecurringJobDto d;



        public UnitOfWork(RsContext db, IStorageConnection hangfireStorage)
        {
            this.db = db;
            this.hangfireStorage = hangfireStorage;
        }




        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
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
            db.SaveChanges();
        }
        private bool IsNotProcessing()
        {
            d = hangfireStorage.GetRecurringJobs().Where(x => x.Id == "DBF_Connector.BackgroundTask").FirstOrDefault();
            var tmp = d?.LastJobState;
            if (tmp != "Processing")
            {
                return true;
            }
            else return false;
        }
        private void CheckNull<T>(params T[] par)
           where T : class
        {
            foreach (var el in par)
            {
                if (el == null)
                    throw new ArgumentNullException($"{nameof(el)} cannot be null") { };
            }
        }
        private IEnumerable<TResult> GetAll<TResult>(DbSet<TResult> set)
            where TResult : DBObject<int>
        {
            return ApplyIe(set, x => x);
        }

        private TResult Get<TResult>(DbSet<TResult> set, Func<TResult, bool> predicate)
            where TResult : DBObject<int>
        {
            return ApplySingle<TResult>(set, x => x.FirstOrDefault(predicate));
        }


        public IEnumerable<TResult> GetAll<TResult>()
           where TResult : DBObject<int>
        {
            return (this as IRepository<TResult>).GetAll();
        }

        IEnumerable<Element> IRepository<Element>.GetAll()
        {
            return GetAll(db.Elements);
        }

        private RetType Apply<EType, RetType>(DbSet<EType> set, Func<DbSet<EType>, RetType> act)
            where EType : class
        {
            CheckNull(set);
            CheckNull(act);
            return act(set);
        }

        private IEnumerable<TResult> ApplyIe<TResult>(DbSet<TResult> set, Func<DbSet<TResult>, IEnumerable<TResult>> act)
            where TResult : DBObject<int>
        {
            return IsNotProcessing() ? Apply<TResult, IEnumerable<TResult>>(set, act) : new List<TResult>();
        }

        private TResult ApplySingle<TResult>(DbSet<TResult> set, Func<DbSet<TResult>, TResult> act)
            where TResult : DBObject<int>
        {
            return IsNotProcessing() ? Apply<TResult, TResult>(set, act) : default(TResult);
        }

        private bool CheckIsNotProcessingApply<EType>(DbSet<EType> set, Func<DbSet<EType>, bool> act)
            where EType : class
        {
            return IsNotProcessing() ? Apply(set, act) : false;
        }

        public Element Get(Func<Element, bool> predicate)
        {
            return ApplySingle(db.Elements, x => x.FirstOrDefault(predicate));
        }

        private IEnumerable<TResult> Filter<TResult>(DbSet<TResult> set, Func<TResult, bool> predicate)
            where TResult : DBObject<int>
        {
            return ApplyIe(set, x => x.Where(predicate));
        }

        public IEnumerable<Element> Filter(Func<Element, bool> predicate)
        {
            return Filter(db.Elements, predicate);
        }

        private bool Create<TCreate>(DbSet<TCreate> set, TCreate item)
            where TCreate : DBObject<int>
        {
            return CheckIsNotProcessingApply(set, x =>
            {
                if (x.FirstOrDefault(y => y.Code == item.Code) == null)
                {
                    x.Add(item);
                    return true;
                }
                else return false;
            });
        }

        public bool Create(Element item)
        {
            return Create(item);
        }

        public bool Update(Element item)
        {
            if (item == null)
                throw new ArgumentNullException("Item cannot be null");

            if (IsNotProcessing())
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
            return CheckIsNotProcessingApply(set, x =>
            {
                if (x.Find(item.Code) != null)
                {
                    x.Remove(item);
                    return true;
                }
                else return false;
            });
        }
        public bool Delete(Element item)
        {
            return Delete(item);
        }

        IEnumerable<Specification> IRepository<Specification>.GetAll()
        {

            return GetAll(db.Specifications);
        }



        public IEnumerable<Specification> Filter(Func<Specification, bool> predicate)
        {
            return Filter(db.Specifications, predicate);
        }

        public bool Create(Specification item)
        {
            return Create(db.Specifications,item);
        }

        public bool Update(Specification item)
        {
            if (item == null)
                throw new ArgumentNullException("Item cannot be null");

            if (IsNotProcessing())
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
            return GetAll(db.SmtBoxes);
        }


        public IEnumerable<Smt_box> Filter(Func<Smt_box, bool> predicate)
        {
            return Filter(predicate);
        }

        private bool BoxCreate(Smt_box item)
        {
            return CheckIsNotProcessingApply(db.SmtBoxes, x =>
            {
                if (x.FirstOrDefault(y => y.Code == item.Code && y.ElementId == item.ElementId) == null)
                {
                    x.Add(item);
                    return true;
                }
                else return false;
            });
        }
        public bool Create(Smt_box item)
        {
            return BoxCreate(item);
        }

        public bool Update(Smt_box item)
        {
            if (item == null)
                throw new ArgumentNullException("Item cannot be null");

            if (IsNotProcessing())
            {
                var tmp = db.SmtBoxes.FirstOrDefault(x => x.ElementId == item.ElementId && x.Code == item.Code);
                if (tmp != null)
                {
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
            return Get(db.Specifications, predicate);
        }

        public Smt_box Get(Func<Smt_box, bool> predicate)
        {
            return Get(db.SmtBoxes, predicate);
        }

        public bool Delete(Specification Item)
        {
            return Delete(db.Specifications, Item);
        }

        public bool Delete(Smt_box Item)
        {
            return Delete(db.SmtBoxes, Item);
        }

        IEnumerable<RsTask> IRepository<RsTask>.GetAll()
        {
            return GetAll(db.GlobalTasks);
        }

        public RsTask Get(Func<RsTask, bool> predicate)
        {
            return Get(db.GlobalTasks, predicate);
        }

        public IEnumerable<RsTask> Filter(Func<RsTask, bool> predicate)
        {
            return Filter(db.GlobalTasks, predicate);
        }

        public bool Create(RsTask item)
        {
            return Create(db.GlobalTasks, item);
        }

        public bool Update(RsTask item)
        {
            CheckNull(item);

            if (IsNotProcessing())
            {
                var tmp = db.GlobalTasks.FirstOrDefault(x => x.Code == item.Code);
                if (tmp != null)
                {
                    tmp.Code = item.Code;
                    tmp.NavProp = item.NavProp;
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

            if (IsNotProcessing())
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

        public IEnumerable<WarehouseTask> GetAll()
        {
            return GetAll(db.WarehouseTasks);
        }

        public WarehouseTask Get(Func<WarehouseTask, bool> predicate)
        {
            return Get(db.WarehouseTasks,predicate);
        }

        public IEnumerable<WarehouseTask> Filter(Func<WarehouseTask, bool> predicate)
        {
            return Filter(db.WarehouseTasks, predicate);
        }

        public bool Create(WarehouseTask item)
        {
            return Create(db.WarehouseTasks, item);
        }

        public bool Update(WarehouseTask item)
        {
            throw new NotImplementedException();
        }

        public bool Delete(WarehouseTask Item)
        {
            return Delete(db.WarehouseTasks, Item);
        }

        IEnumerable<SmtLineTask> IRepository<SmtLineTask>.GetAll()
        {
            return GetAll(db.SmtLineTasks);
        }

        public SmtLineTask Get(Func<SmtLineTask, bool> predicate)
        {
            return Get(db.SmtLineTasks, predicate);
        }

        public IEnumerable<SmtLineTask> Filter(Func<SmtLineTask, bool> predicate)
        {
            return Filter(db.SmtLineTasks, predicate);
        }

        public bool Create(SmtLineTask item)
        {
            return Create(db.SmtLineTasks, item);
        }

        public bool Update(SmtLineTask item)
        {
            throw new NotImplementedException();
        }

        public bool Delete(SmtLineTask Item)
        {
            return Delete(db.SmtLineTasks, Item);
        }
    }
}
