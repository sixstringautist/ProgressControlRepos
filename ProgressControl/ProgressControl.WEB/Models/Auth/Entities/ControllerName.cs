using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProgressControl.DAL.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProgressControl.WEB.Models.Auth.Entities
{
    public class ControllerName : RefCollectionEntity<ActionName,int>
    {
        [Key]
        public override int Code { get; set; }
        public string Name { get; set; }

    }
}