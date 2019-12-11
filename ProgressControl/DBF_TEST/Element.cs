using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBF_TEST
{
    public class Element : TwoRefCollectionEntity<ElementQuantity, Analogs, int>
    {
        private string _name;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int Code { get; set; }
        public string Name { get => _name; set => _name = value.Trim(' ', '*'); }
        public int Quantity { get; set; }
        public Element()
        {
            Collection = new List<ElementQuantity>();
            CollectionTwo = new List<Analogs>();
        }
    }
}
