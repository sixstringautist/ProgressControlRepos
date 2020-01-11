using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProgressControl.DAL.Entities;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ProgressControl.WEB_New_.Models
{
    public class PlanViewModel
    { 
        public int Code { get; set; }
        public string State { get; set; }
        public DateTime CreationTime { get; set; }
        public ICollection<SubtaskViewModel> Subtasks { get; set; }
        public int SelectedSpecificationCode { get; set; }
        public PlanViewModel()
        {
            Subtasks = new List<SubtaskViewModel>();
        }
    }


    public class SubtaskViewModel
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public int Quantity { get; set; }


        public static explicit operator SubtaskViewModel(Subtask sbtsk)
        {
            return new SubtaskViewModel()
            {
                Code=sbtsk.Code,
                Name = sbtsk.Specification.Name,
                State = sbtsk.WorkState.ToString(),
                Quantity = sbtsk.Quantity
            };
        }
    }


    public class SubtaskCreateViewModel
    {
        public SelectList SpecificationsList { get; set; }
    }

    public class SpcViewModel
    {
        public int Code { get; set; }
        public string Name { get; set; }
    }
}