using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressControl.DAL.Entities
{
    public abstract class AbstractTask<TRef, TDetails> : RefCollectionEntity<TRef,int>,ITask
        where TRef : class
    {
        [Key]
        public override int Code { get; set; }
        public abstract DateTime CreationTime { get; protected set; }
        public abstract DateTime LastPauseTime { get; protected set; }
        public abstract DateTime LastStartTime { get; protected set; }
        public abstract DateTime CompleteTime { get; protected set; }

        public abstract State WorkState { get; protected set; }

        public abstract bool Complete();
        public abstract bool Pause();
        public abstract bool Start();

        public Details<TDetails> Details { get; set; }


        protected abstract bool CanComplete();
        public abstract void AddToDone(object obj);


    }


    public class Details<T> : DBObject<int>
    {
        public override int Code { get; set; }
        public virtual ICollection<T> Subtasks { get; set; }

    }

    public class SubtaskDetails : Details<AreaTask>
    {

        public int TaskId { get; set; }

        public AreaTask Task {get; set;}

        public int SubtaskId { get; set; }
        public Subtask Subtask { get; set; }


        public SubtaskDetails()
        { }

    }

    //TODO: Смотрю сюда, тебе надо сообразить как получать состояние задачи участка

    public class Subtask : AbstractTask<AreaTask,SubtaskDetails>
    {


        public override State WorkState { get; protected set; }
        public int Priority { get; set; }

        public int SpecificationId { get; set; }
        public Specification Specification { get; private set; }

        public event Action UpdateParent
        {
            add 
            {
                UpdateParent += value;
            }
            remove
            {
                UpdateParent += value;
            } 
        }


        public int TaskId { get; private set; }
        public RsTask Task { get; set; }
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
            Need = new List<Details>();
            Left = new List<Details>();
        }
        public Subtask(int need) : this()
        {
            var tmp = new Details(Collection.First().Code, need,this);
            
            Need.Add(tmp);
            Left.Add(tmp);
        }

        public override void AddToDone(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj cannot be null");
            if (obj.GetType() != typeof(int))
                throw new ArgumentException($"Invalid dype");
            done += Math.Abs((int)obj);
            Left.First().Quantity = Need.First().Quantity - done;
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
    

    public class AreaTask : Subtask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Code { get; set; }
        public int AreaId { get; set; }
        public virtual RsArea Area { get; set; }

        public AreaTask() : base()
        {
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
            Need.Remove(Need.Single(x => x.ElementId == id));
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
                Need.Add(new Details(,));
            }
        }

        public override List<Details> Left { get; protected set; }

        private void SetLeft()
        {
            Need.ForEach(x => 
            {
                Left.Add(new Details(x.ElementId, x.Quantity));
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




        protected override bool CanComplete()
        {
            if (Left.First().Quantity <= 0)
                return true;
            else return false;
        }
    }
    #endregion

}
