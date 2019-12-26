using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressControl.DAL.Entities
{
    public enum State
    {
        InProcess,
        Complete,
        Paused
        
    }
    //TODO: Задачи участков и их подзадачи(пул задач)
    public class RsTask : RefCollectionEntity<Subtask,int>
    {
        public DateTime CreationTime { get; private set; }

        public State State { get; set; }
        public RsTask()
        {
            Collection = new List<Subtask>();
            CreationTime = DateTime.Now;
        }
    }
}
