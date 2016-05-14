using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sys = Cosmos.System;

using SharpOS.SystemRing;

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
            env_vars += ";VERSION:" + kernel_version + ";KERNEL:" + kernel_flavour;
            FS = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(FS);
            FS.Initialize();
            Console.WriteLine("Scanning filesystems...");
            Console.Clear();
            Console.WriteLine("Welcome to SharpOS.");
            InterpretCMD("$VERSION");
            InterpretCMD("$KERNEL");
            InterpretCMD("$AUTHOR");
            Console.Beep(50, 100);
            Console.Beep(100, 100);
            Console.Beep(150, 100);

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
                    MessageBox(".NET Exception!", e.Message);
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
            else if(lower.StartsWith("help"))
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
                    Console.WriteLine("<DIR>\t" + dir);
                }
                foreach (var dir in Directory.GetFiles(current_directory))
                {
                    string[] sp = dir.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    Console.WriteLine(sp[sp.Length - 1] + "\t" + dir);
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
                if(!Directory.Exists(current_directory + dir))
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
                MessageBox(title, text);
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
            bool val = true;
            foreach (var dir in Directory.GetDirectories(path))
            {
                val = (path == dir);
            }
            return val;
        }

        public string[] split_str(string subject, string split)
        {
            return subject.Split(new[] { split }, StringSplitOptions.RemoveEmptyEntries);
        }

        public void MessageBox(string title, string text)
        {
            List<string> message_lines = new List<string>();
            message_lines.Add("+------[ " + title + " ]------+");
            string t = message_lines[0];
            int h_space = t.Length - 2;
            message_lines.Add("|" + Repeat(" ", h_space) + "|");
            int h_pad = h_space - 2;
            foreach (string c in SplitChunks(text, h_pad))
            {
                message_lines.Add("| " + c + Repeat(" ", c.Length - h_pad) + " |");
            }
            message_lines.Add("|" + Repeat(" ", h_space) + "|");
            string button = "[enter:ok] ";
            int blength = button.Length;
            int bpad = (h_space - blength) - 1;
            message_lines.Add("|" + Repeat(" ", bpad) + button + " |");
            message_lines.Add("+" + Repeat("-", h_space) + "+");
            DrawCenter(message_lines.ToArray());
            var k = Keyboard.ReadKey();
            if (k.Key == "enter")
            {
                Console.CursorLeft = 0;
                Console.CursorTop = 0;
                Console.Clear();
            }
        }

        public void DrawCenter(string[] contents)
        {
            int cornerx = (Console.WindowWidth - contents[0].Length) / 2;
            int cornery = (Console.WindowHeight - contents.Length) / 2;
            for (int i = 0; i < contents.Length; i++)
            {
                Console.CursorLeft = cornerx;
                Console.CursorTop = cornery + i;
                Console.Write(contents[i]);
            }
            

        }

        public string Repeat(string text, int length)
        {
            string t = text;
            while (text.Length <= length)
            {
                text += t;
            }
            return text;
        }

        public string[] SplitChunks(string orig, int size)
        {
            if (string.IsNullOrEmpty(orig))
            {
                throw new ArgumentNullException("orig");
            }

            if (size <= 0)
            {
                throw new ArgumentException("Size of chunk must be above 0.");
            }

            List<string> chunks = new List<string>();
            while (orig.Length >= size)
            {
                chunks.Add(orig.Substring(0, size));
                orig = orig.Remove(0, size);
            }
            if (orig.Length > 0)
            {
                chunks.Add(orig);
            }
            return chunks.ToArray();
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
    }
}
