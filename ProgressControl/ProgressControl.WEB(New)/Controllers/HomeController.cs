using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Providers;
using System.Web.Security;
using ProgressControl.WEB.Models;
using ProgressControl.WEB_New_.Model.Repositories;
using ProgressControl.DAL.EF;
using Hangfire;
namespace ProgressControl.WEB.Controllers
{
    public class HomeController : Controller
    {
        private UnitOfWork u;
        
        public HomeController()
        {
            u = new UnitOfWork(DependencyResolver.Current.GetService<RsContext>(), JobStorage.Current.GetConnection());
        }

        public ActionResult Index()
        {
            return View();  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Required]LogInViewModel user)
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                u.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}