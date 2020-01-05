using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProgressControl.DAL.Entities;
using System.ComponentModel.DataAnnotations;
namespace ProgressControl.WEB_New_.Models
{
    public class PlaneViewModel
    { 
        public int Code { get; set; }
        public string State { get; set; }
        public DateTime CreationTime { get; set; }
        public ICollection<SubtaskViewModel> Subtasks { get; set; }
    }


    public class SubtaskViewModel
    {
        public string Name { get; set; }
        public string State { get; set; }
    }


}