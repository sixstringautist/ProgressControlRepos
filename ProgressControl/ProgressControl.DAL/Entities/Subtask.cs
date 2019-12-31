using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ProgressControl.DAL.Entities
{
    public abstract class AbstractTask<TRef> : RefCollectionEntity<TRef,int>,ITask
        where TRef : class
    {
        public abstract DateTime CreationTime { get; protected set; }
        public abstract DateTime LastPauseTime { get; protected set; }
        public abstract DateTime LastStartTime { get; protected set; }
        public abstract DateTime CompleteTime { get; protected set; }

        public abstract State WorkState { get; protected set; }

        public abstract bool Complete();
        public abstract bool Pause();
        public abstract bool Start();

        public virtual List<Details> Need { get; protected set; }
        public virtual List<Details> Left { get; protected set; }


        protected abstract bool CanComplete();
        public abstract void AddToDone(object obj);

    }

    public struct Details
    {
        public int ElementId { get; private set; }
        public int Quantity { get; set; }
        public Details(int id, int quantity)
        {
            ElementId = id;
            Quantity = quantity;
        }
    }

    public class Subtask : AbstractTask<Specification>
    {

        int done;


        public override State WorkState { get; protected set; }
        public int Priority { get; set; }

        



        public int TaskId { get; private set; }
        public Task Task { get; set; }
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
        public Subtask(int need) : this()
        {
            var tmp = new Details(Collection.First().Code, need);
            Need = new List<Details>();
            Need.Add(tmp);
            Left = new List<Details>();
            Left.Add(tmp);
        }

        public override void AddToDone(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj cannot be null");
            if (obj.GetType() != typeof(int))
                throw new ArgumentException($"Invalid dype");
            done += Math.Abs((int)obj);
            Left.Clear();
            int left = Need.First().Quantity - done;
            Left.Add(new Details(Need.First().ElementId,  left < 0 ? 0 : left ));
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
            if (Left.First().Quantity == 0)
                return true;
            return false;
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
    #region AreaTasks
    public abstract class AreaTask : Subtask
    {
        public int AreaId { get; set; }
        public RsArea Area { get; set; }
        public ICollection<DBObject<int>> TaskWarehouse { get; private set; }

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

    }
  

    public class WarehouseTask : AreaTask
    {
       
        protected override bool CanComplete()
        {
            if (Left.Count == 0)
            {
                return true;
            }
            return false;
        }

        public void RemoveBox(int id)
        {
            TaskWarehouse.Remove(TaskWarehouse.Single(x => x.Code == id));
        }
        public override void AddToDone(object obj)
        {
            Smt_box box;
            if (obj == null)
                throw new ArgumentNullException("obj cannot be null");
            if (obj is Smt_box)
                box = obj as Smt_box;
            else throw new ArgumentException("Invalid parameter type");

            if (Left.Count != 0)
            {
                AddBox(box);
            }
        }

        public WarehouseTask()
        {
            SetNeed();
        }

        public override List<Details> Need { get; protected set; }

        private void SetNeed()
        {
            foreach (var el in Collection.First().Collection)
            {
                Need.Add(new Details(el.Code, el.Quantity));
            }
        }

        private void AddBox(Smt_box box)
        {
            if (Collection.First().Collection.FirstOrDefault(x => x.Code == box.ElementId) != null
                && TaskWarehouse.FirstOrDefault(x => x.Code == box.Code) == null)
            {
                TaskWarehouse.Add(box);
                SetLeft();
            }
        }

        public override List<Details> Left { get; protected set; }

        private void SetLeft()
        {
            Need.ForEach(x => 
            {
                var exists = TaskWarehouse.Where(y => { return (y as Smt_box).ElementId == x.ElementId ? true : false; }).Cast<Smt_box>().ToList();
                var sum = exists.Sum(z => z.Quantity);
                var _left = Need.Single(z => z.ElementId == x.ElementId).Quantity - sum;
                if (_left > 0)
                {
                    if (!Left.Exists(el => el.ElementId == x.ElementId))
                        Left.Add(new Details(x.ElementId, _left));
                    else
                    {
                        Left.Remove(Left.Find(e => e.ElementId == x.ElementId));
                        Left.Add(new Details(x.ElementId, _left));
                    }
                }
            });

        }

       

        

       
    }

    public class SmtLineTask : AreaTask
    {
        public override DateTime CreationTime { get; protected set; }
        public override DateTime LastPauseTime { get; protected set; }
        public override DateTime LastStartTime { get; protected set; }
        public override DateTime CompleteTime { get; protected set; }

        public override State WorkState { get; protected set; }

        public override List<Details> Need { get; protected set; }
        public override List<Details> Left { get; protected set; }
        int done;



        protected override bool CanComplete()
        {
            if (Left.First().Quantity <= 0)
                return true;
            else return false;
        }
    }
    #endregion

}
