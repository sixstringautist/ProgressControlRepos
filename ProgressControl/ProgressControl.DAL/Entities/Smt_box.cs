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
    public class Smt_box
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(name:"BoxId",Order = 0)]
        public int Smt_boxId { get; private set; }

        public bool InFeeder { get; set; }

        public int CurrentQuantity => Quantity - Spent;
        public int Spent { get; set; }

        public int Quantity { get; set; }

        [Key]
        [ForeignKey("Element")]
        [Column(name:"ElementId", Order = 1)]
        public int ElementId { get; set; }
        public virtual Element Element { get; set; }

        public DateTime CreationDate { get; set; }

        public Smt_box()
        {
            CreationDate = DateTime.Now;
        }
    }
}
