using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ProgressControl.DAL.Entities
{
    public abstract class AreaTask : DBObject<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Code { get; set; }
        public DateTime CreationTime { get; protected set; }
        public DateTime LastPauseTime { get; protected set; }
        public DateTime LastStartTime { get; protected set; }
        public DateTime CompleteTime { get; protected set ; }
        public State WorkState { get; protected set; }

        public int SubtaskId { get; set; }
        public virtual Subtask Subtask { get; protected set; }

        public int ContainerId { get; protected set; }
        public virtual Container Container { get; protected set; }

        public AreaTask()
        {
            CreationTime = DateTime.Now;
            LastPauseTime = DateTime.MinValue;
            LastStartTime = DateTime.MinValue;
            CompleteTime = DateTime.MinValue;
            WorkState = State.New;
        }
        protected abstract bool CanComplete();
        public bool Complete()
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
        public bool Start()
        {
            if (WorkState == State.InProcess)
                return false;
            WorkState = State.InProcess;
            LastStartTime = DateTime.Now;
            return true;
        }
        public bool Pause()
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
    }

    public class WarehouseTask : AreaTask
    {
        public int AreaId { get; private set; }
        public virtual WarehouseArea Area { get; private set; }

        public override Subtask Subtask { get; protected set; }

        private WarehouseTask() : base()
        {
        }
        public WarehouseTask(Subtask tsk, WarehouseArea warehouse, Container c):base()
        {
            Subtask = tsk;
            SubtaskId = tsk.Code;
            Area = warehouse;
            AreaId = warehouse.Code;
            base.Container = c;
        }


        public IEnumerable<(int, string, int)> GetNeed()
        {
            var tmp = new List<(int, string, int)>();
            foreach (var el in Subtask.Specification.Collection)
            {
                var exists = Container.Elements?.Where(x => x.Code == el.Code);
                var Sum = exists == null ? 0 : exists.Sum(x => x.CurrentQuantity);
                var left = (el.Quantity * Subtask.Quantity) - Sum;
                if (left > 0)
                {
                    tmp.Add((el.Code, el.NavProp.Name, left));
                }
            }
            return tmp;
        }
        protected override bool CanComplete()
        {
            foreach (var el in Subtask.Specification.Collection)
            {
                var tmp = Container.Elements.Where(x => x.Code == el.Code) as Smt_box;
                if (tmp == null)
                    return false;
                if (tmp.BoxQuanttity - el.Quantity > 0)
                    return false;
            }
            return true;
        }
    }
    public class SmtLineTask : AreaTask
    {
        public int AreaId { get; private set; }
        public virtual SmtLineArea Area { get; private set; }

        public override Subtask Subtask { get => base.Subtask; protected set => base.Subtask = value; }

        private SmtLineTask() { }
        public SmtLineTask(Subtask tsk, SmtLineArea line, Container c) : base()
        {
            Subtask = tsk;
            SubtaskId = tsk.Code;
            Area = line;
            AreaId = line.Code;
            base.Container = c;
        }
        protected override bool CanComplete()
        {
            return false;
        }
    }
}
