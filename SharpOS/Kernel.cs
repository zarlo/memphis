using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sys = Cosmos.System;

using SharpOS.SystemRing;
using System.Drawing;
using ConsoleDraw.Windows;

namespace SharpOS
{
    public class Kernel : Sys.Kernel
    {
        string env_vars = "TEST:A test value;AUTHOR:Michael VanOverbeek";

        string current_directory = "0:\\";

        const string kernel_version = "0.0.1";
        const string kernel_flavour = "Earth";



        protected override void BeforeRun()
        {
            KernelUtils.Init(this);
            Curse.kern = this;
            env_vars += ";VERSION:" + kernel_version + ";KERNEL:" + kernel_flavour;
            FS = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(FS);
            FS.Initialize();
            Console.WriteLine("Scanning filesystems...");
            Console.Clear();
            Console.WriteLine("Welcome to SharpOS.");
            /*InterpretCMD("$VERSION");
            InterpretCMD("$KERNEL");
            InterpretCMD("$AUTHOR");
            Console.Beep(50, 100);
            Console.Beep(100, 100);
            Console.Beep(150, 100);
            */
            Console.WriteLine("System dir separator: " + Sys.FileSystem.VFS.VFSManager.GetDirectorySeparatorChar());
            
        }

        bool running = true;
        public Cosmos.System.FileSystem.CosmosVFS FS = null;
        protected override void Run()
        {
            while (running)
            {
                try
                {
                    Console.Write(current_directory + "> ");
                    string input = Console.ReadLine();
                    InterpretCMD(input);
                }
                catch(Exception e)
                {
                    Curse.ShowMessagebox(".NET Exception!", e.Message);
                }
            }
        }

        public void InterpretCMD(string input)
        {
            string lower = input.ToLower(); //so commands are not case-sensitive use this
            if (lower.StartsWith("shutdown"))
            {
                Console.Clear();
                Console.WriteLine("It is safe to shut down your system.");
                running = false;
            }
            else if(lower.StartsWith("curse_test"))
            {
                var w = new ConsoleDraw.Windows.Base.Window(2, 2, Console.WindowWidth - 2, Console.WindowHeight - 2, null);
                var a = new Alert("Test.", w);
                a.MainLoop();
            }
            else if (lower.StartsWith("sharppad"))
            {
                string fname = "";
                try
                {
                    fname = current_directory + input.Remove(0, 9);
                }
                catch
                {

                }
                StartApplicationLoop(new Apps.SharpPad(), new[] { fname });
            }
            else if (lower.StartsWith("fileskimmer"))
            {
                StartApplicationLoop(new Apps.FileSkimmer(), new[] { " " });
            }
            else if(lower.StartsWith("pkg_install "))
            {
                string p = input.Remove(0, 12);
                if(Directory.Exists(p))
                {
                    KernelUtils.HandlePackageInstall(p);
                }
            }
            else if(lower.StartsWith("write "))
            {
                string fname = lower.Remove(0, 6);
                if(string.IsNullOrEmpty(fname))
                {
                    throw new Exception("write - Trying to write to nothing, I see?");
                }
                Console.WriteLine("Single-Line Writer - Writing to file " + fname);
                Console.WriteLine("You can use C#-like escape sequences like '\\n' for new-lines and '\\t' for tabs. If you want to actually write a visible escape string - i.e 'This sequence, \\n, will print a newline', you can type in '\\\\' to escape the backslash.");
                Console.Write("> ");
                string text = Console.ReadLine();
                //escape sequence parser
                text = text.Replace("\\\\", "===");
                text = text.Replace("\\n", "\n");
                text = text.Replace("\\t", "\t");
                text = text.Replace("===", "\\"); //so that the above 2 methods don't get confused
                if(!File.Exists(current_directory + fname))
                {
                    var f = File.Create(current_directory + fname);
                    f.Close();
                }
                File.WriteAllText(current_directory + fname, text);
            }
            else if (lower.StartsWith("help "))
            {
                string topic = lower.Remove(0, 5);
                StartApplicationLoop(new Apps.HelpViewer(), new[] { "0:\\help.db", topic });
            }
            else if (lower.StartsWith("help"))
            {
                StartApplicationLoop(new Apps.HelpViewer(), new[] { "0:\\help.db" });
            }
            else if (lower.StartsWith("$"))
            {
                string[] evars = split_str(env_vars, ";");
                foreach (string kv in evars)
                {
                    string[] var = split_str(kv, ":");
                    if (var[0].ToLower() == lower.Remove(0, 1))
                    {
                        Console.WriteLine(var[0] + " = " + var[1]);
                    }
                }
            }
            else if (lower.StartsWith("test_crash"))
            {
                throw new Exception("Test crash.");
            }
            else if (lower.StartsWith("reboot"))
            {
                Sys.Power.Reboot();
            }
            else if (lower.StartsWith("clear"))
            {
                Console.Clear();
            }
            else if (lower.StartsWith("echo "))
            {
                try
                {
                    Console.WriteLine(input.Remove(0, 5));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("echo: " + ex.Message);
                }
            }
            else if (lower.StartsWith("dir"))
            {
                Console.WriteLine("Type\tName");
                foreach (var dir in Directory.GetDirectories(current_directory))
                {
                    try
                    {
                        Console.WriteLine("<DIR>\t" + dir);
                    }
                    catch
                    {

                    }
                }
                foreach (var dir in Directory.GetFiles(current_directory))
                {
                    try
                    {
                        string[] sp = dir.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                        Console.WriteLine(sp[sp.Length - 1] + "\t" + dir);
                    }
                    catch
                    {

                    }
                }

            }
            else if(lower.StartsWith("test_write"))
            {
                var f = File.Create(current_directory + "TestFile.txt");
                f.Close();
                File.WriteAllText(current_directory + "TestFile.txt", "Test\nAnother Test");
                Console.WriteLine("Created file!");
            }
            else if(lower.StartsWith("mkdir "))
            {
                string dir = input.Remove(0, 6);
                if(!dir_exists(current_directory + dir))
                {
                    Directory.CreateDirectory(current_directory + dir);
                }
                else
                {
                    Console.WriteLine("mkdir: Directory exists.");
                }
                
            }
            else if(lower.StartsWith("cd "))
            {
                var newdir = input.Remove(0, 3);
                if(dir_exists(newdir))
                {
                    Directory.SetCurrentDirectory(current_directory);
                    current_directory = current_directory + newdir + "\\";
                }
                else
                {
                    if(newdir == "..")
                    {
                        var dir = FS.GetDirectory(current_directory);
                        string p = dir.mParent.mName;
                        if(!string.IsNullOrEmpty(p))
                        {
                            current_directory = p;
                        }
                    }
                }
            }
            else if (lower.StartsWith("print "))
            {
                string file = input.Remove(0, 6);
                    if (File.Exists(current_directory + file))
                    {
                        Console.WriteLine(File.ReadAllText(current_directory + file));
                    }
                    else
                    {
                        Console.WriteLine("print: File doesn't exist.");
                    }
                
            }
            else if (lower.StartsWith("lsvol"))
            {
                var vols = FS.GetVolumes();
                Console.WriteLine("Name\tSize\tParent");
                foreach (var vol in vols)
                {
                    Console.WriteLine(vol.mName + "\t" + vol.mSize + "\t" + vol.mParent);
                }
            }
            else if(lower.StartsWith("msg"))
            {
                Console.Write("Title: ");
                string title = Console.ReadLine();
                Console.Write("Message Text: ");
                string text = Console.ReadLine();
                Curse.ShowMessagebox(title, text);
            }
            else if(lower.StartsWith("scr "))
            {
                string p = input.Remove(0, 4);
                Interpret_Script(current_directory + p);
            }
            else
            {
                Console.WriteLine("Invalid Command.");
            }
             
        }

        public void Interpret_Script(string path)
        {
            string[] lines = File.ReadAllLines(path);
            foreach(string line in lines)
            {
                InterpretCMD(line);
            }
        }

        public bool dir_exists(string path)
        {
            bool val = false;
            foreach (var dir in Directory.GetDirectories(current_directory))
            {
                if(path == dir)
                {
                    val = true;
                }
            }
            return val;
        }

        public string[] split_str(string subject, string split)
        {
            return subject.Split(new[] { split }, StringSplitOptions.RemoveEmptyEntries);
        }

        
        public void StartApplicationLoop(IApplication app, string[] args)
        {
            app.Start(args);
            while(app.Running)
            {
                app.MainLoop(); //while app is running, loop this method.
            }
            app.End(); //stop application, returning control to the kernel.
        }

        public string get_current_dir()
        {
            return current_directory;
        }

        public void set_current_dir(string d)
        {
            current_directory = d;
        }
    }

    public class KernelUtils
    {
        private static Kernel kern = null;

        public static void Init(Kernel k)
        {
            kern = k;
        }

        public static string GetCurrentDir()
        {
            return kern.get_current_dir();
        }

        public static void SetCurrentDir(string d)
        {
            kern.set_current_dir(d);
        }

        public static void HandlePackageInstall(string path)
        {
            if(File.Exists(path + "\\package.apm"))
            {
                string[] ln = File.ReadAllLines(path + "\\package.apm");
                string source = "";
                string dest = "";
                foreach (var line in ln)
                {
                    if (line.ToLower().StartsWith("Source="))
                    {
                        source = line.Remove(0, 7).Replace("%this%", path);
                    }
                    else if (line.ToLower().StartsWith("Dest="))
                    {
                        source = line.Remove(0, 5).Replace("%main%", "0:\\");
                    }
                }
                CopyRecursive(source, dest);
            }
            else
            {
                throw new Exception("package.apm file not found! Invalid package.");
            }
        }

        public static void CopyRecursive(string s, string d)
        {
            foreach (var file in Directory.GetFiles(s))
            {
                byte[] fbytes = File.ReadAllBytes(s + "\\" + file);
                File.WriteAllBytes(d + "\\" + file, fbytes);
            }
            foreach(var dir in Directory.GetDirectories(s))
            {
                if(!Directory.Exists(d + "\\" + dir))
                {
                    Directory.CreateDirectory(d + "\\" + dir);
                    CopyRecursive(s + "\\" + dir, d + "\\" + dir);
                }
            }
        }
    }
}
