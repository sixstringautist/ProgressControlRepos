using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressControl.DAL.Entities
{
    public interface IAreaTaskGenerator
    {
        void GenerateTasks(Subtask subtask);
    }


    public abstract class AbstractGenerator : DBObject<int>, IAreaTaskGenerator
    {
        [ForeignKey("Area")]
        public override int Code { get; set; }
        public virtual RsArea Area { get; protected set; }
        public abstract void GenerateTasks(Subtask subtask);
    }

    public class WarehouseGenerator : AbstractGenerator,IAreaTaskGenerator
    {
        private WarehouseGenerator()
        { 
        }

        public WarehouseGenerator(WarehouseArea area):this()
        {
            Area = area;
        }
        
        public override void GenerateTasks(Subtask subtask)
        {
            var tmp = new WarehouseTask(subtask, Area , subtask.Container);
            subtask.AreaTasks.Add(tmp);
            (Area as WarehouseArea).WarehouseTasks.Add(tmp);
        }
    }

    public class SmtLineGenerator : AbstractGenerator, IAreaTaskGenerator
    {
        private SmtLineGenerator()
        { }
        public SmtLineGenerator(SmtLineArea area)
        {
            Area = area;
        }
        
        public override void GenerateTasks(Subtask subtask)
        {
            var tmp = new SmtLineTask(subtask, Area, subtask.Container);
            subtask.AreaTasks.Add(tmp);
            (Area as SmtLineArea).SmtLineTasks.Add(tmp);
        }
    }
}
