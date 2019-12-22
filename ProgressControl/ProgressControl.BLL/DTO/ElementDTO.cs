using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgressControl.DAL.Entities;
namespace ProgressControl.BLL.DTO
{
    class ElementDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public uint Quantity { get; set; }

        public ICollection<ElementQuantityDTO> Specifications { get; set; }
        public ICollection<AnalogDTO> Parents { get; set; }
        public ICollection<AnalogDTO> Childs { get; set; }
    }
}
