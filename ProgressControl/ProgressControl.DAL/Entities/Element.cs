using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressControl.DAL.Entities
{
    public class Element : RefCollectionEntity<ElementQuantity,int>, ISelfReferenceCollection<Analog>
    {

        private string _name;


        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int Code { get; set; }

        public string Name { get => _name; set => _name = value.Trim(' ', '*'); }

        public int Quantity { get; set; }

        public string Un { get; set; }



        public virtual ICollection<Analog> Parents { get; set; }
        public virtual ICollection<Analog> Childrens { get; set; }


        public virtual ICollection<AreaTask> Task { get; set; }

        public virtual ICollection<Smt_box> Boxes { get; set; }


        public Element()
        {
            Collection = new List<ElementQuantity>();
            Parents = new List<Analog>();
            Childrens = new List<Analog>();
            Boxes = new List<Smt_box>();
            //Task = null;
        }

    }

}
