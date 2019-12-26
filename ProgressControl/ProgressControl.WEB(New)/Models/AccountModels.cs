using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ProgressControl.WEB.Models
{
    public class LogInViewModel
    {
        [Required]
        [Display(Name ="Введите логин")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Введите пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordRetry { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Patronimyc { get; set; }

        public SelectList Roles { get; set; }

    }
}