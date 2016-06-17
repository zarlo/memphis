using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpOS.Apps
{
    //TODO: Make appscape compatible with sharpos.
    class Appscape_Mini : IApplication
    {
        public override void Start(string[] args)
        {
            throw new NotImplementedException();
        }

        public override bool Running
        {
            get; set;
        }

        public override void MainLoop()
        {
            throw new NotImplementedException();
        }

        public override void End()
        {
            throw new NotImplementedException();
        }
    }
}
