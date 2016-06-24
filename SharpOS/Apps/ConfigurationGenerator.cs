using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memphis.Apps
{
    class ConfigurationGenerator : IApplication
    {
        public override bool Running
        {
            get; set;
        }

        private bool file_generated = false;

        public override void End()
        {
            if (file_generated == true)
            {
                Console.Clear();
                Running = false;
            }
            else
            {
                Running = true;
            }
        }
        
        public override void MainLoop()
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Clear();
            TUI.Utils.ClearArea(0, 0, Console.WindowWidth, 1, ConsoleColor.Gray);
            Console.CursorTop = 0;
            Console.CursorLeft = 1;
            Console.WriteLine("Memphis Installer");
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.CursorLeft = 1;
            Console.CursorTop = 2;
            SetupStatus();
        }

        public void SetupStatus()
        {
            Console.Write(" == " + statustext + " ==");
            Console.CursorLeft = 1;
            Console.CursorTop += 2;
            if (status == 0)
            {
                Console.Write(@"Welcome to Memphis. Memphis is a C# operating system
 built by Michael VanOverbeek. It's goal is to, well, we don't
 really know.

 Anyways, we've got a few questions for you. Once we're done, 
 we'll have everything ready for you. These questions and
 your answers are crucial to the way Memphis adapts to the
 way you use your computer, and the way your system is
 configured.

 Note that Memphis isn't for inexperienced or new
 PC users. You need to know what a FAT partition is,
 what a partition scheme is, and what the 'Master
 Boot Record' is.

 Do you know for sure that your hard drive has a Master Boot Record,
 and you have at least one Cosmos-compatible FAT partition on your drive? [y/n]");
                var inf = Console.ReadKey();
                if(inf.Key == ConsoleKey.Y)
                {
                    status += 1;
                    statustext = "Installation Phase 1: Directory Structure";
                }
                else if(inf.Key == ConsoleKey.N)
                {
                    Curse.ShowMessagebox("Memphis can't run properly on this system.", "Memphis needs at least one FAT partition on a Master Boot Record to be able to store it's configuration and other files on. Please use a partition utility like GParted to partition your hard drive properly.");
                    file_generated = true;
                    End();   
                }
            }
            else if(status == 1)
            {
                Console.Write(@"Now it's time to set up your system's directory structure.

 If you have a FAT partition it should have already been mounted to 0:\ at boot.
 If it exists, we'll continue. If not, the installer will close.

 Press any key to continue.");
                try
                {
                    Console.ReadKey();
                }
                catch
                {

                }
                if(!Directory.Exists(@"0:\"))
                {
                    Curse.ShowMessagebox("FAT partition on 0:\\ not found.", "Memphis could not find a valid partition on 0:\\. Please use a partition tool like GParted to partition your drive properly.");
                    file_generated = true;
                    End();
                }
                else
                {
                    Console.WriteLine("0:\\ was found, starting directory creation...");
                    foreach(var path in directories)
                    {
                        CreateDir("0:\\" + path);
                    }
                    Console.WriteLine("Done. Press a key.");
                    try
                    {
                        Console.ReadKey();
                    }
                    catch
                    {

                    }
                    status += 1;
                    statustext = "Installation Phase 2: Personalization.";
                }
            }
        }

        public void CreateDir(string path)
        {
            if(!Directory.Exists(path)) {
                Console.WriteLine("Created: " + path);
                Directory.CreateDirectory(path);
            }
        }

        private string[] directories = { "docs", "conf", "conf\\memphis", "conf\\skin", "temp", "pkg", "cil"};

        public int status = 0;
        public string statustext = "Status";

        public override void Start(string[] args)
        {
            Console.Clear();
            file_contents = new List<string>();
            statustext = "Welcome to Memphis";
            Running = true;
        }

        public List<string> file_contents = new List<string>();
    }
}
