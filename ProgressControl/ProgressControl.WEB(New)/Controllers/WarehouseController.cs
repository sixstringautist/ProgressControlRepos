using Hangfire;
using PagedList;
using ProgressControl.DAL.EF;
using ProgressControl.DAL.Entities;
using ProgressControl.WEB_New_.Model.Repositories;
using ProgressControl.WEB_New_.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
namespace ProgressControl.WEB_New_.Controllers
{


    [System.Web.Mvc.Authorize(Roles = "user")]
    public class WarehouseController : Controller
    {
        UnitOfWork u;
        public WarehouseController()
        {
            u = new UnitOfWork(DependencyResolver.Current.GetService<RsContext>(), JobStorage.Current.GetConnection());
        }
        // GET: Warehouse
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var tmp = u.GetAll<Smt_box>().ToList();
            List<Smt_BoxViewModel> model = new List<Smt_BoxViewModel>();
            foreach (var el in tmp)
                model.Add(new Smt_BoxViewModel()
                {
                    BoxCode = el.Code,
                    BoxQuantity = el.BoxQuanttity,
                    ElCode = el.ElementId,
                    CreationTime = el.CreationDate,
                    ElementName = el.Element.Name,
                    Spent = el.Spent
                });

            return View(model.ToPagedList(pageNumber, pageSize));
        }
        [OutputCache(Duration = 20, Location = System.Web.UI.OutputCacheLocation.ServerAndClient)]
        public ActionResult Elements(int? page)
        {
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var Elements = u.GetAll<Element>().ToList();
            ViewBag.Elements = Elements;
            var ElementsView = new List<ElementViewModel>();
            Elements.ForEach(x => ElementsView.Add(x));
            return View(ElementsView.ToPagedList(pageNumber, pageSize));
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult CreateBox(int? id)
        {
            if (id == null)
                return RedirectToAction("Index");
            var tmp = u.Get(delegate (Element x) { return x.Code == id; });
            if (tmp != null)
            {

                var box = new Smt_box(tmp);
                box.ElementId = tmp.Code;
                tmp.Boxes.Add(box);
                if (u.Create(box))
                {
                    u.Save();
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Tasks(int? page)
        {
            int pageSize = 20;
            int pageNumber = (page ?? 1);

            List<WarehouseTaskView> list = new List<WarehouseTaskView>();
            var tmp = u.GetAll<WarehouseTask>().AsQueryable()
                .Include(x => x.Subtask)
                .Include(x => x.Container)
                .Include(x => x.Container.Elements).ToList();
            tmp.ForEach(x=> list.Add(x));
            Session["list"]=list.ToPagedList(pageNumber, pageSize);
            return View(Session["list"]);
        }

        public ActionResult ElementsPager(int? page, int Id)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            var tmp = (IPagedList<WarehouseTaskView>)Session["list"];
            var list = tmp.Single(x => x.Id == Id).Need;
            ViewBag.TaskId = Id;
            return PartialView("AreaTaskPartial", list.ToPagedList(pageNumber, pageSize));
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult SaveChanges([FromBody]Smt_ViewBoxModelChange model)
        {

            var tmp = u.Get(delegate (Smt_box el) { return el.FullCode == model.FullCode; });
            if (tmp != null)
            {
                tmp.BoxQuanttity = model.BoxQuantity;
                u.Save();
                return Json(true, JsonRequestBehavior.DenyGet);
            }
            else return Json(false, JsonRequestBehavior.DenyGet);
        }
    }
}