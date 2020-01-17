using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressControl.DAL.Entities
{
    public interface IAreaTaskGenerator
    {
        void GenerateTasks(Subtask subtask);
    }


    public abstract class AbstractGenerator : DBObject<int>, IAreaTaskGenerator
    {
        public override int Code { get; set; }
        public abstract void GenerateTasks(Subtask subtask);
    }

    public class WarehouseGenerator : AbstractGenerator,IAreaTaskGenerator
    {
        public override int Code { get; set; }
        public int Areaid { get; protected set; }
        public WarehouseArea Area { get; protected set; }

        public WarehouseGenerator(WarehouseArea area)
        {
            this.Area = area;
        }
        
        public override void GenerateTasks(Subtask subtask)
        {
            var tmp = new WarehouseTask(subtask, Area);
            subtask.WarehouseTasks.Add(tmp);
            Area.Tasks.Add(tmp);
        }
    }

    public class SmtLineGenerator : AbstractGenerator, IAreaTaskGenerator
    {
        public override int Code { get; set; }
        public int AreaId { get; protected set; }
        public SmtLineArea Area { get; protected set; }

        public SmtLineGenerator(SmtLineArea area)
        {
            this.Area = area;
        }
        
        public override void GenerateTasks(Subtask subtask)
        {
            var tmp = new SmtLineTask(subtask, Area);
            subtask.SmtLineTasks.Add(tmp);
            Area.Tasks.Add(tmp);
        }
    }
}
