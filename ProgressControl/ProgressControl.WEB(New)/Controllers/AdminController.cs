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
using ProgressControl.WEB_New_.Models.TaskManager;
namespace ProgressControl.WEB_New_.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        UnitOfWork u;
        RsTaskManager manager;
        public AdminController()
        {
           u = new UnitOfWork(DependencyResolver.Current.GetService<RsContext>(), JobStorage.Current.GetConnection());
            manager = new RsTaskManager(u);
        }
        public ActionResult Index()
        {
            var specifications = u.GetAll<Specification>();
            var SelectList = new SelectList(specifications, "Code", "Name");
            ViewBag.spc = SelectList;
            ViewData["SelectedItem"] = new List<SelectListItem>();
            return View("Plans",Plans());
        }


        private List<PlanViewModel> Plans()
        {
            var model = new List<PlanViewModel>();
            var tmp = u.GetAll<RsTask>();
            foreach (var el in tmp)
            {
                model.Add(new PlanViewModel() { CreationTime = el.CreationTime, State = el.WorkState.ToString(), Code = el.Code });
            }
            model.ForEach(x =>
            {
                var el = tmp.Single(y => y.Code == x.Code);
                el.Subtasks.ToList().ForEach(y =>
                {
                    x.Subtasks.Add(new SubtaskViewModel() {Code =y.Code ,Name = y.Specification.Name, State = y.WorkState.ToString(),Quantity = y.Quantity});
                });
            });

            return model;
        }

        [HttpPost]
        public ActionResult CreatePlane()
        {
            var tmp = manager.CreatePlan();
            ViewBag.Data = tmp;
            var specifications = u.GetAll<Specification>();
            var SelectList = new SelectList(specifications, "Code", "Name");
            ViewBag.spc = SelectList;
            ViewData["SelectedItem"] = new List<SelectListItem>();
            return PartialView("CreatePlanePartial",new PlanViewModel());
        }

        [HttpPost]
        public ActionResult CreateSubtaskForPlane(int planId,int spcId, int quantity )
        {
            SubtaskViewModel model = (SubtaskViewModel)manager.AddSubtaskToPlan((uint)planId, (uint)spcId, (uint)quantity) ?? new SubtaskViewModel();

            ViewData["PlanId"] = planId;
            return PartialView("CreateSubtask",model);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                u.Dispose();
                manager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}