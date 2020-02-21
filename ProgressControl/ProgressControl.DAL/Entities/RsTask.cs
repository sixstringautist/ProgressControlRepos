using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgressControl.DAL.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProgressControl.DAL.Entities
{
    public enum State
    {
        InProcess = 1,
        Complete,
        Paused,
        New = 0
    }

    public delegate void CompleteHandler();

    public abstract class AbstractTask<TRef> : OneReferenceEntity<TRef, int>, ITask
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

        abstract public event CompleteHandler CompleteEvent;

        protected abstract bool CanComplete();
    }

    public interface ITask
    {
        DateTime CreationTime { get; }
        DateTime LastPauseTime { get; }
        DateTime LastStartTime { get; }
        DateTime CompleteTime { get; }
        bool Start();
        bool Pause();
        bool Complete();
    }

    public class RsTask :AbstractTask<RsTask>
    {
        private DateTime _creationTime;

        private event CompleteHandler completeEvent;
        public override event CompleteHandler CompleteEvent
        {
            add { if (value != null) completeEvent += value; }
            remove { if (value != null) completeEvent -=value; }
        }

        public override DateTime CreationTime { get => _creationTime; protected set => _creationTime = value; }

        public override DateTime LastPauseTime { get; protected set; }

        public override DateTime LastStartTime { get; protected set; }

        public override DateTime CompleteTime { get; protected set; }

        public override State WorkState { get; protected set; }

        private ICollection<Subtask> sbtsk;
        public virtual ICollection<Subtask> Subtasks { get => sbtsk; 
            protected set
            {
                if (value != null)
                {
                    foreach (var el in value)
                    {
                        el.CompleteEvent += OnComplete;
                    }
                    sbtsk = value;
                }   
            }
        }

        [NotMapped]
        public override int NavPropId { get => base.NavPropId; set => base.NavPropId = value; }
        [NotMapped]
        public override RsTask NavProp { get => base.NavProp; set => base.NavProp = value; }



        public RsTask()
        {
            CreationTime = DateTime.Now;
            LastPauseTime = DateTime.MinValue;
            LastStartTime = DateTime.MinValue;
        }
        public RsTask( List<Subtask> l):this()
        {
            this.Subtasks = l;
        }
        public override bool Start()
        {
            if (WorkState == State.InProcess)
                return false;
            WorkState = State.InProcess;
            LastStartTime = DateTime.Now;
            return true;
        }

        private void OnComplete()
        {
            this.Complete();
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


        public override bool Complete()
        {
            switch (WorkState)
            {
                case State.Complete:
                    return false;
                case State.InProcess:
                    if (CanComplete())
                    {
                        WorkState = State.InProcess;
                        CompleteTime = DateTime.Now;
                        return true;
                    }
                    return false;
                case State.New:
                    return false;
                case State.Paused:
                    return false;
                default:
                    return false;
            }
        }

        protected override bool CanComplete()
        {
            foreach (var el in sbtsk)
            {
                if (el.WorkState != State.Complete)
                    return false;
            }
            return false;
        }
    }
}
