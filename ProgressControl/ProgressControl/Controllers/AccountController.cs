using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProgressControl.DAL.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ProgressControl.Models;
using ProgressControl.DAL.Entities;
using System.Threading.Tasks;
using ProgressControl.DAL.Repositories;

namespace ProgressControl.Controllers
{
    public class AccountController : Controller
    {


        public ActionResult Register()
        {
            return View();
        }
    }
}
