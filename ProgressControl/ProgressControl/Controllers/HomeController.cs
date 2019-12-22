﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProgressControl.DAL.Interfaces;
using ProgressControl.DAL.EF;
using ProgressControl.DAL.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ProgressControl.Controllers
{
    public class HomeController : Controller
    {
        IUnitOfWork unit;

        ApplicationContext cnt;
        //ApplicationRoleManager roleManager;

        ApplicationContext AppContext
        {
            get
            {
                return cnt ?? HttpContext.GetOwinContext().Get<ApplicationContext>();
            }
        }

        public HomeController(IUnitOfWork u, ApplicationContext cnt)
        {
            unit = u;
            this.cnt = cnt;
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}