using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProgressControl.DAL.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProgressControl.WEB.Models.Auth.Entities
{
    public class ActionName : OneReferenceEntity<ControllerName,int>
    {
        [Key]
        public override int Code { get; set; }
        public ICollection<Role> Collection { get; set; }
        [ForeignKey("NavProp")]
        public override int NavPropId { get; set; }

    }
}