using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModels
{
    public class Smt_box : Element
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Smt_boxId { get; set; }

        public int BoxQuantity { get; set; }

        public DateTime CreationDate { get; set; }


        public Smt_box()
        {
            CreationDate = DateTime.Now;
        }
    }
}
