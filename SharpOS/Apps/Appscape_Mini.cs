﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memphis.Apps
{
    //TODO: Make appscape compatible with Memphis.
    class Appscape_Mini : IApplication
    {
        public override void Start(string[] args)
        {
            Curse.ShowMessagebox("Appscape", "Appscape is not currently available in this version of Memphis.");
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
