using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressControl.DAL.Entities
{
    public class Smt_box : DBObject<int>
    {
        [Key, Column(name: "BoxId", Order = 1), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Code { get; set; }

        [Key]
        [Column(name: "ElementId", Order = 0)]
        [ForeignKey("Element")]
        public int ElementId { get; set; }
        private Element element;
        public virtual Element Element { get => element;
            protected set
            {
                element = value;
            }
        }

        public bool InFeeder { get; set; }
        public bool InComplect { get; set; }

        public int BoxQuanttity { get; set; }

        [NotMapped]
        public string FullCode { get => ElementId + " " + Code; }

        [NotMapped]
        public int CurrentQuantity => BoxQuanttity - Spent;
        public int Spent { get; set; }

        public int DischargeLosses { get; set; }

        public int? ContainerId { get; set; }
        public virtual Container Container { get; protected set; }


        public virtual ICollection<BoxHistory> HistoryPoints { get; set; }

        public DateTime CreationDate { get; set; }


        public Smt_box()
        {
            CreationDate = DateTime.Now;
        }
        public Smt_box(Element el) : this(new List<BoxHistory> ())
        {
            Element = el;
        }
        private Smt_box(List<BoxHistory> hist)
        {
            this.HistoryPoints = hist;
        }
    }
}
