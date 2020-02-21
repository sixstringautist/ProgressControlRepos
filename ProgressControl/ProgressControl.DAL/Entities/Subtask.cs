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

        public override State WorkState { get; protected set; }
        public int Priority { get; set; }

        public int Quantity { get; private set; }
        public int SpecificationId { get; set; }
        public virtual Specification Specification { get; private set; }

        private ICollection<SmtLineTask> smtLineTasks;
        public virtual ICollection<SmtLineTask> SmtLineTasks { get => smtLineTasks;
            protected set
            {
                if (value != null)
                {
                    foreach (var el in value)
                    {
                        el.CompleteEvent += OnComplete;
                    }
                    smtLineTasks = value;
                }
            }
        }

        private ICollection<WarehouseTask> warehouseTasks;
        public virtual ICollection<WarehouseTask> WarehouseTasks { get => warehouseTasks;
            protected set
            {
                if (value != null)
                {
                    foreach (var el in value)
                    {
                        el.CompleteEvent += OnComplete;
                    }
                    warehouseTasks = value;
                }
            }
        }


        public virtual Container Container { get; set; }

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
        }
        public Subtask(Specification spc, int quantity) : this()
        {
            this.Specification = spc;
            this.Quantity = quantity;
            Container = new Container(new List<Smt_box>(), this);
            SmtLineTasks = new List<SmtLineTask>();
            WarehouseTasks = new List<WarehouseTask>();
        }

        private event CompleteHandler completeEvent;
        public override event CompleteHandler CompleteEvent 
        {
            add 
            {
                if (value != null)
                    completeEvent += value;
            } 
            
            remove
            {
                if (value != null)
                    completeEvent += value;
            } 
        }

        public void OnComplete()
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
            foreach (var el in SmtLineTasks)
            {
                if (el.WorkState != State.Complete)
                    return false;
            }
            foreach (var el in WarehouseTasks)
            {
                if (el.WorkState != State.Complete)
                    return false;
            }
            return true;
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
                        completeEvent?.Invoke();
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

    public class Container : DBObject<int>
    {
        [Key]
        public override int Code { get; set; }
        public virtual Subtask NavProp { get; set; }
        public virtual ICollection<Smt_box> Elements { get; protected set; }
        public void AddElement(Smt_box el)
        {
            if (!Elements.Contains(el))
            {
                (el as Smt_box).InComplect = true;
                Elements.Add(el);
            }
        }

        public Container(ICollection<Smt_box> boxes, Subtask sbtsk)
        {
            Elements = boxes;
            NavProp = sbtsk;
        }

        private Container()
        {

        }


    }

  
    #endregion

}
