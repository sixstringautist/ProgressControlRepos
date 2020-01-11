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
        public WarehouseArea()
        {
            WarehouseTasks = new List<WarehouseTask>();
            base.Generator = new WarehouseGenerator(this);
        }
        public ICollection<WarehouseTask> WarehouseTasks { get; set; }
    }

    public class SmtLineArea : RsArea
    {
        public SmtLineArea()
        {
            SmtLineTasks = new List<SmtLineTask>();
            base.Generator = new SmtLineGenerator(this);
        }
        public ICollection<SmtLineTask> SmtLineTasks { get; set; }
    }
}
