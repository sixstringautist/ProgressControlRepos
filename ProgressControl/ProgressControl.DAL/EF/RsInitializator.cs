using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ProgressControl.DAL.Entities;

namespace ProgressControl.DAL.EF
{
    class RsInitializator : DropCreateDatabaseAlways<RsContext>
    {

        protected override void Seed(RsContext context)
        {
            base.Seed(context);
            var Smt = new SmtLineArea(new List<SmtLineTask>()) { Name = "смт" };
            var Warehouse = new WarehouseArea(new List<WarehouseTask>()) { Name = "склад" };
            Smt.Generator = new SmtLineGenerator(Smt);
            Warehouse.Generator = new WarehouseGenerator(Warehouse);
            context.RsAreas.Add(Smt);
            context.RsAreas.Add(Warehouse);
        }
    }
}
