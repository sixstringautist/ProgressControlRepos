using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DBF_TEST
{
    class ElementQuantity : ManyToManyRelation<Element, Specification, int, int>
    {
        [Key, ForeignKey("NavPropTwo")]
        [Column("Spcode", Order = 0)]
        public override int Code { get; set; }
        [Key, ForeignKey("NavProp")]
        [Column("Elcode", Order = 1)]
        public override int CodeTwo {get;set;}
        public int Quantity { get; set; }
        
    }
}
