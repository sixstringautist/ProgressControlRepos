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
        public virtual AbstractGenerator Generator { get; set; }

    }

    public class WarehouseArea : RsArea
    {

        public virtual ICollection<WarehouseTask> WarehouseTasks { get; set; }
        private WarehouseArea()
        {
        }
        public WarehouseArea(List<WarehouseTask> list):this()
        {
            this.WarehouseTasks = list;
        }
        

    }

    public class SmtLineArea : RsArea
    {
        public virtual ICollection<SmtLineTask> SmtLineTasks { get; set; }
        private SmtLineArea()
        {

        }
        public SmtLineArea(List<SmtLineTask> list):this()
        {
            this.SmtLineTasks = list;
        }
        
    }
}
