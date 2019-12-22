using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressControl.BLL.DTO
{
    class SpecificationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }

        public ICollection<ElementDTO> ElementsDTO { get; set; }
    }
}
