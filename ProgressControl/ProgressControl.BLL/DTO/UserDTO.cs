using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressControl.BLL.DTO
{
    class UserDTO
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronimyc { get; set; }
        public string FullName => string.Join(" ", LastName, FirstName, Patronimyc);
        public string Role { get; set; }
    }
}
