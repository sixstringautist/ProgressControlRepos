using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DomainModels
{
    public class Analog : ManyToManyRelation<Element, Element,int,int>
    {
        
        public override int Code { get; set; }
        public override int CodeTwo { get; set; }
        public int AnalogPriority { get; set; }
    }
}
