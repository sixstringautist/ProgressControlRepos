using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DBF_TEST
{
    public class Analogs : ManyToManySelfRelation<Element, int>
    {
        [Key,Column("code",Order =0)]
        public override int Code { get; set; }
        [Required]
        [Key,ForeignKey("NavProp"),Column("acode",Order = 1)]
        public int ACode { get; set; }

    }
}
