using System.Collections.Generic;
using ProgressControl.DAL.Entities;
namespace ProgressControl.WEB.Models.Auth.Entities
{
    public class Role : RefCollectionEntity<User, int>
    {
        public string RoleName { get; set; }

        public Role()
        {
            Collection = new List<User>();
        }
    }
}