using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressControl.DAL.Entities
{
    public interface IAreaTaskGenerator
    {
        void GenerateTasks(Subtask subtask, Container c);
    }


    public abstract class AbstractGenerator : DBObject<int>, IAreaTaskGenerator
    {
        public override int Code { get; set; }
        public abstract void GenerateTasks(Subtask subtask, Container c);
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
        
        public override void GenerateTasks(Subtask subtask, Container c)
        {
            var tmp = new WarehouseTask(subtask, Area, c);
            c.Collection.Add(tmp);
            Area.WarehouseTasks.Add(tmp);
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
        
        public override void GenerateTasks(Subtask subtask, Container c)
        {
            var tmp = new SmtLineTask(subtask, Area, c);
            c.Collection.Add(tmp);
            Area.SmtLineTasks.Add(tmp);
        }
    }
}
