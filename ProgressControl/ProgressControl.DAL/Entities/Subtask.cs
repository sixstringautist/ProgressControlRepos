﻿using System;
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

        public int SpecificationId { get; set; }
        public virtual Specification Specification { get; private set; }

        public virtual ICollection<AreaTask> AreaTasks { get; private set; }


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

    public class Container : OneReferenceEntity<AreaTask, int>
    {
        [Key]
        public override int Code { get; set; }
        public virtual ICollection<Smt_box> Elements { get; protected set; }
        public void AddElement(Smt_box el)
        {
            if (!Elements.Contains(el))
            {
                (el as Smt_box).InComplect = true;
                Elements.Add(el);
            }
        }
        public Container()
        {
            Elements = new List<Smt_box>();
        }
    }

  
    #endregion

}