using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

//TODO: Подумай над виртуальными складами
namespace ProgressControl.DAL.Entities
{
    public class Subtask : OneReferenceEntity<Specification, int>
    {
        public int SpecificationId { get; set; }

        [Range(1,int.MaxValue)]
        public int Need { get; set; }
        public int Done { get; set; }
        public int Left => Need - Done;
        [Display(Name = "Статус")]
        public State State { get; set; }

        bool IsInSmtLine { get; set; }

        public int TaskId { get; set; }
        public virtual Task Task { get; set; }
    }
}
