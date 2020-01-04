using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressControl.DAL.Entities
{
    [Table("Smt_Boxes")]
    public class Smt_box : Element
    {
        [Key, Column(name:"BoxId", Order = 1),DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Code { get;  set; }

        [Key]
        [Column(name: "ElementId", Order = 0)]
        [ForeignKey("Element")]
        public  int ElementId { get; set; }
        public Element Element { get; protected set; }

        public bool InFeeder { get; set; }
        public bool InComplect { get; set; }

        public int BoxQuanttity { get; set; }

        [NotMapped]
        public int CurrentQuantity => BoxQuanttity - Spent;
        public int Spent { get; set; }

        public int DischargeLosses { get; set; }

        public int ContainerId { get; set; }
        public virtual Container Container { get; set; }

        public DateTime CreationDate { get; set; }



        public Smt_box()
        {
            CreationDate = DateTime.Now;
        }
    }
}
