using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections;

namespace DBF_TEST
{
    public class Specification : RefCollectionEntity<ElementQuantity, int>
    {
        private string _name;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int Code { get; set; }
        [Required]
        public string Name { get => _name; set => _name = value.Trim(' '); }
        [Required]
        public DateTime Date { get; set; }
        public Element this[int index]
        {
            get
            {
                return Collection.FirstOrDefault(x => x.CodeTwo == index).NavProp;
            }
        }
        public int this[Element index]
        {
            get
            {
                return Collection.FirstOrDefault(x => x.CodeTwo == index.Code).CodeTwo;
            }
        }



        public Specification() { Collection = new List<ElementQuantity>(); }

        public IEnumerator GetEnumerator()
        {
            return Collection.GetEnumerator();
        }
    }
}
