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
            context.RsAreas.Add(new WarehouseArea(new List<WarehouseTask>()) { Name="Склад"});
            context.RsAreas.Add(new SmtLineArea(new List<SmtLineTask>()) { Name = "Линия SMT" });

        }
    }
}
