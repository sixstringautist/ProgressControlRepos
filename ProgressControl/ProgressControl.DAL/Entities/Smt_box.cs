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
        [Key, Column(name:"BoxId", Order = 1)]
        public override int Code { get;  set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(name: "ElementId", Order = 0)]
        public  int ElementId { get; set; }

        public bool InFeeder { get; set; }

        public int CurrentQuantity => Quantity - Spent;
        public int Spent { get; set; }

        public int DischargeLosses { get; set; }


        public DateTime CreationDate { get; set; }

        public Smt_box()
        {
            CreationDate = DateTime.Now;
        }
    }
}
