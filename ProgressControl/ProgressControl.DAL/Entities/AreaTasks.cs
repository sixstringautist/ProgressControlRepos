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
        public Subtask Subtask { get; protected set; }

        public int ContainerId { get; protected set; }
        public Container Container { get; protected set; }

        public AreaTask()
        {
            CreationTime = DateTime.Now;
            LastPauseTime = DateTime.MinValue;
            LastStartTime = DateTime.MinValue;
            CompleteTime = DateTime.MinValue;
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

    [Table("WarehouseTasks")]
    public class WarehouseTask : AreaTask
    {
        public WarehouseArea WarehouseArea { get; private set; }

        public IEnumerable<(int, string, int)> GetNeed()
        {
            var tmp = new List<(int, string, int)>();
            foreach (var el in Subtask.Specification.Collection)
            {
                var exists = Container.Elements.Where(x => x.Code == el.Code);
                var Sum = exists.Sum(x => x.CurrentQuantity);
                var left = el.Quantity - Sum;
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
    [Table("SmtLineTasks")]
    public class SmtLineTask : AreaTask
    {
        public SmtLineArea SmtLineArea { get; private set; }

        protected override bool CanComplete()
        {
            return false;
        }
    }
}
