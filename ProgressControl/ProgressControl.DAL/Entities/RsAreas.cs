using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProgressControl.DAL.Entities
{
    public abstract class RsArea : DBObject<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Code { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public AbstractGenerator Generator { get; protected set; }

    }

    public class WarehouseArea : RsArea
    {
        public ICollection<WarehouseTask> Tasks { get; private set; }
        private WarehouseArea()
        {
            base.Generator = new WarehouseGenerator(this);
        }

        public WarehouseArea(List<WarehouseTask> tasks)
        {
            Tasks = tasks;
        }
    }

    public class SmtLineArea : RsArea
    {
        public ICollection<SmtLineTask> Tasks { get; private set; }
        public SmtLineArea()
        {
            base.Generator = new SmtLineGenerator(this);
        }
        public SmtLineArea(List<SmtLineTask> tasks)
        {
            Tasks = tasks;
        }
    }
}
