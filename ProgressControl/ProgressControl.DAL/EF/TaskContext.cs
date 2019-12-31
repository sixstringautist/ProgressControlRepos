using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ProgressControl.DAL.Entities;

namespace ProgressControl.DAL.EF
{
    class TaskContext : DbContext
    {
        public DbSet<RsTask> GlobalTasks;

        public DbSet<Subtask> SubTasks;


    }
}
