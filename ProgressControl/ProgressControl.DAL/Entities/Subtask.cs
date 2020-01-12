using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressControl.DAL.Entities
{

    public class Subtask : AbstractTask<RsTask>
    {

        class AreaTaskComparer : IComparer<AreaTask>
        {
            public int Compare(AreaTask x, AreaTask y)
            {
                return x.Code.CompareTo(y);
            }
        }


        public override State WorkState { get; protected set; }
        public int Priority { get; set; }

        public int Quantity { get; private set; }
        public int SpecificationId { get; set; }
        public virtual Specification Specification { get; private set; }

        public virtual Container Container { get; set; }

        public delegate void CompleteHandler();
        private event CompleteHandler complete;
        public event CompleteHandler CompleteEvent
        {
            add
            {
                if (value != null)
                    complete += value;
            }
            remove
            {
                if (value != null)
                    complete -= value;
            }
        }
        public int Top{ get; set; }
        List<AreaTask> areaTasks;
        public virtual ICollection<AreaTask> AreaTasks { get => areaTasks; 
            private set 
            {
                areaTasks = value?.ToList();
                areaTasks.ForEach(x => x.CompleteEvent += this.OnComplete);
            } 
        }



        public override DateTime CreationTime { get; protected set; }
        public override DateTime LastPauseTime { get; protected set; }
        public override DateTime LastStartTime { get; protected set; }
        public override DateTime CompleteTime { get; protected set; }

        public Subtask()
        {
            CreationTime = DateTime.Now;
            WorkState = State.New;
            LastPauseTime = DateTime.MinValue;
            LastStartTime = DateTime.MinValue;
            CompleteTime = DateTime.MinValue;
            Top = 0;
        }
        public Subtask(Specification spc,int quantity):this()
        {
            this.Specification = spc;
            this.Quantity = quantity;
            areaTasks = new List<AreaTask>();
            Container = new Container(new List<Smt_box>(), areaTasks, this);
        }



        private void OnComplete()
        {
            this.Complete();
        }

        public override bool Start()
        {
            if (WorkState == State.InProcess)
                return false;
            WorkState = State.InProcess;
            LastStartTime = DateTime.Now;
            return true;
        }

        public override bool Pause()
        {
            switch (WorkState)
            {
                case State.Paused:
                    return false;
                case State.Complete:
                    return false;
                case State.New:
                    return false;
                case State.InProcess:
                    WorkState = State.Paused;
                    LastPauseTime = DateTime.Now;
                    return true;
                default:
                    return false;
            }
        }

        protected override bool CanComplete()
        {
            foreach (var el in AreaTasks)
            {
                if (el.WorkState != State.Complete)
                    return false;
            }
            if (Top == areaTasks.IndexOf(areaTasks.Last()))
                return true;
            else return false;
        }

        public override bool Complete()
        {
            switch (WorkState)
            {
                case State.Complete:
                    return false;
                case State.InProcess:
                    if (CanComplete())
                    {
                        WorkState = State.Complete;
                        CompleteTime = DateTime.Now;
                        complete?.Invoke();
                        return true;
                    }
                    else return false;
                case State.New:
                    return false;
                case State.Paused:
                    return false;
                default:
                    return false;
            }
        }

    }
  


   

    

    //TODO: Смотрю сюда, тебе надо сообразить как получать состояние задачи участка


    #region AreaSubTasks

    public class Container : RefCollectionEntity<AreaTask, int>
    {
        [Key]
        public override int Code { get; set; }
        public virtual ICollection<Smt_box> Elements { get; protected set; }

      
        public virtual Subtask Subtask { get; set; }

        public Container(ICollection<Smt_box> boxes, List<AreaTask> tsks, Subtask sbtsk)
        {
            Elements = boxes;
            Collection = tsks;
            Subtask = sbtsk;
        }

        private Container()
        {

        }


    }

  
    #endregion

}
