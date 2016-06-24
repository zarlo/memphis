using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memphis
{
    public abstract class IApplication
    {
        public abstract void Start(string[] args);
        
        public abstract bool Running { get; set; }

        public abstract void MainLoop();

        public abstract void End();
    }
}
