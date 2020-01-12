using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProgressControl.DAL.Entities;
using ProgressControl.WEB_New_.Model.Repositories;
using System.Data.Entity;
namespace ProgressControl.WEB_New_.Models.TaskManager
{
    public class RsTaskManager: IDisposable
    {
        private UnitOfWork u;

        public RsTaskManager( UnitOfWork u)
        {
            this.u = u;
        }

        public RsTask CreatePlan()
        {
            var tmp = new RsTask(new List<Subtask>());
            if (u.Create(tmp))
            {
                u.Save();
                return tmp;
            }
            return null;
        }
        public Subtask AddSubtaskToPlan(uint planId,uint spcId, uint Quantity)
        {
            var tmp = u.Get(delegate(RsTask tsk) { return tsk.Code == planId;});
            if (tmp == null)
                return null;
            var spc = u.Get(delegate (Specification sp) { return sp.Code == spcId; });
            if (spc == null)
                return null;
            var sbtsk = new Subtask(spc, (int)Quantity);
            sbtsk.NavProp = tmp;
            tmp.Subtasks.Add(sbtsk);
            var smt = u.GetAll<RsArea>().AsQueryable().OfType<WarehouseArea>().Include(x => x.WarehouseTasks).Include(x=> x.Generator).Single();
            var warehouse = u.GetAll<RsArea>().AsQueryable().OfType<SmtLineArea>().Include(x => x.SmtLineTasks).Include(x=> x.Generator).Single();

            smt.Generator.GenerateTasks(sbtsk);
            warehouse.Generator.GenerateTasks(sbtsk);

            u.Save();
          
            return sbtsk;
        }

        public void AddBoxToContainer(uint AreaTaskId, uint BoxId)
        {
            var Task = u.Get(delegate (AreaTask a) { return a.Code == AreaTaskId; }) as WarehouseTask;
            var Box = u.Get(delegate (Smt_box b) { return b.Code == BoxId; });
            if (Task == null || Box == null )
                return;
            Task.AddElement(Box);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                u.Dispose();
            }
        }
        //TODO:Реализовать менеджер
    }
}