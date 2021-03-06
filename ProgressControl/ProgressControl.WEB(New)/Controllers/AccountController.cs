﻿using ProgressControl.WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Providers.Entities;
using System.Web.Security;
using MyEntities = ProgressControl.WEB.Models.Auth.Entities;

namespace ProgressControl.WEB.Controllers
{

    public class AccountController : Controller
    {
        // GET: Accounts
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = Membership.CreateUser(model.Username, model.Password) as MyEntities.User;
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Patronimyc = model.Patronimyc;
                    Membership.Provider.UpdateUser(user as MembershipUser);

                    Roles.AddUserToRole(model.Username, "user");

                }
                else
                {
                    ModelState.AddModelError("", "Не удалось создать пользователя");
                }
            }
            return View("Index", model);
        }
    }
}