using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProgressControl.DAL.Entities;
using ProgressControl.WEB_New_.Model.Repositories;
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
            var cont = new Container(new List<Smt_box>(), new List<AreaTask>());
            u.GetAll<RsArea>().ToList().ForEach(x => x.Generator.GenerateTasks(sbtsk, cont));
            u.Save();
          
            return sbtsk;
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