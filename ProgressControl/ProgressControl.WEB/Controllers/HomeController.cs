using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Providers;
using System.Web.Security;
using pgProvider;
using ProgressControl.WEB.Models;
namespace ProgressControl.WEB.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();  
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LogInViewModel user)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(user.Username,user.Password))
                {
                    FormsAuthentication.SetAuthCookie(user.Username, true);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "NotFound");
                }
            }
            return View("Index",user);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}