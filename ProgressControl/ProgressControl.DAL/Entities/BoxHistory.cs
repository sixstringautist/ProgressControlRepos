using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressControl.DAL.Entities
{
    public class BoxHistory : OneReferenceEntity<Smt_box, int>
    {

        public DateTime TimePoint { get; protected set; }


        public int WasInBox { get; protected set; }

        public int Spent { get; set; }


        private BoxHistory()
        {
        }

        public BoxHistory(Smt_box b, DateTime t)
        {
            NavProp = b;
            TimePoint = t;
        }
    }
}
