using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DBF_TEST
{
    class Analogs : ManyToManySelfRelation<Element, int>
    {
        [Key]
        public override int Code { get; set; }
        [Required]
        public int ACode { get; set; }

    }
}
