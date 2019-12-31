﻿using System;
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

    public abstract class RsArea : DBObject<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Code { get; set; }

        public string Name { get; set; }

        public abstract ICollection<AreaTask> AreaTasks { get; set; }
        public abstract ICollection<AreaRelation> Childs { get; set; }
        public abstract ICollection<AreaRelation> Parents { get; set; }

    }


    public class AreaRelation : ManyToManyRelation<RsArea, RsArea, int, int>
    {
        public override int Code { get; set; }
        public override int CodeTwo { get; set; }
    }

    public class WarehouseArea : RsArea
    {
        public override ICollection<AreaTask> AreaTasks { get; set; }
        public override ICollection<AreaRelation> Childs { get; set; }
        public override ICollection<AreaRelation> Parents { get; set; }
    }
    public class SmtArea : RsArea
    {

        public override ICollection<AreaTask> AreaTasks { get ; set; }
        public override ICollection<AreaRelation> Childs { get; set; }
        public override ICollection<AreaRelation> Parents { get; set; }
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
    public class RsTask :AbstractTask<Subtask>, ITask
    {
        private DateTime _creationTime;

        public override DateTime CreationTime { get => _creationTime; protected set => _creationTime = value; }

        public override DateTime LastPauseTime { get; protected set; }

        public override DateTime LastStartTime { get; protected set; }

        public override DateTime CompleteTime { get; protected set; }

        public override State WorkState { get; protected set; }


        public RsTask()
        {
            Collection = new List<Subtask>();
            CreationTime = DateTime.Now;
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


        public override bool Complete()
        {
            switch (WorkState)
            {
                case State.Complete:
                    return false;
                case State.InProcess:
                    WorkState = State.InProcess;
                    CompleteTime = DateTime.Now;
                    return true;
                case State.New:
                    return false;
                case State.Paused:
                    return false;
                default:
                    return false;
            }
        }


        //TODO:Определить эти методы
        protected override bool CanComplete()
        {
            throw new NotImplementedException();
        }

        public override void AddToDone(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
