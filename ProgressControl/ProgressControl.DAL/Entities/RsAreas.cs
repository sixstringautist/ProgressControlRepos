using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProgressControl.DAL.Entities
{
    public class RsArea : DBObject<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Code { get; set; }

        public string Name { get; set; }
    }

    public class WarehouseArea : RsArea
    {
        public ICollection<WarehouseTask> WarehouseTasks { get; set; }
    }

    public class SmtLineArea : RsArea
    {
        public ICollection<SmtLineTask> SmtLineTasks { get; set; }
    }
}
