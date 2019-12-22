using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressControl.DAL.Interfaces
{
    public interface IBackGrounder : IDisposable
    {
        void BackgroundTask();
    }
}
