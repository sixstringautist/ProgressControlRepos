using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProgressControl.WEB_New_.Model.Repositories;
using ProgressControl.WEB_New_.Models;
using ProgressControl.DAL.Entities;
using System.Web.Security;
using ProgressControl.DAL.EF;
using Hangfire;

namespace ProgressControl.WEB_New_.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        UnitOfWork u;

        public AdminController()
        {
           u = new UnitOfWork(DependencyResolver.Current.GetService<RsContext>(), JobStorage.Current.GetConnection());
        }

        public ActionResult Index()
        {
            return View("Plans",Plans());
        }


        private List<PlaneViewModel> Plans()
        {
            var model = new List<PlaneViewModel>();
            var tmp = u.GetAll<RsTask>().ToList();
            tmp.ForEach(x => model.Add(new PlaneViewModel() { CreationTime = x.CreationTime, State = x.WorkState.ToString(),Code=x.Code }));
            model.ForEach(x =>
            {
                tmp.Single(y => y.Code == x.Code)
                .Subtasks.ToList().ForEach(y =>
                {
                    x.Subtasks.Add(new SubtaskViewModel() { Name = y.Specification.Name, State = y.WorkState.ToString()});
                });
            });

            return model;
        }

        [HttpGet]
        public ActionResult CreatePlane()
        {
            return PartialView("CreatePlanePartial");
        }

        //[HttpPost]
        //public ActionResult CreatePlane(PlaneViewModel model)
        //{

        //}
    }
}