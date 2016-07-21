using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sys = Cosmos.System;

using Memphis.SystemRing;
using System.Drawing;
using Memphis_SystemRing;
using static Memphis_SystemRing.SystemHelpers;

namespace Memphis
{
    public class User
    {
        public string Name { get; set; }
        public int PermissionLevel { get; set; }
        public string UserDirectory { get; set; }
    }

    public class UserManager
    {
        private static readonly User livesession_user = new User { Name = "memphis_live", PermissionLevel = 3, UserDirectory = "0:\\" }; //Live session user, has admin privileges.
        private static readonly User system_user = new User { Name = "memphis", PermissionLevel = 4, UserDirectory = "0:\\" }; //This user will act as a fully-privileged user to manage the system.
        private static User current_user = null;

        public static User CurrentUser
        {
            get
            {
                if (is_livesession)
                    return livesession_user;
                return current_user;
            }
        }

        private const bool is_livesession = true;

        /// <summary>
        /// Determines if the specified password is correct
        /// for the specified user.
        /// </summary>
        /// <param name="user">The user info to check.</param>
        /// <param name="pass">The password.</param>
        /// <returns>True if the password is correct, false if it wasn't or the password database wasn't found.</returns>
        public static bool Authenticate(User user, string pass)
        {
            if(File.Exists(@"0:\System\passwd"))
            {
                string[] passwd = File.ReadAllLines(@"0:\System\passwd");
                foreach(string line in passwd)
                {
                    string[] data = line.Split('\t');
                    if (data[0] == user.Name && data[1] == pass)
                        return true; 
                }
            }
            return false;
        }

        public static void LoginScreen()
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Clear();
            Console.WriteLine(Kernel.memphis_logo);
            if (is_livesession)
            {
                int seconds_start = SystemInfo.Time.Second;
                int seconds_passed = 0;
                while (seconds_passed < 10)
                {
                    Console.CursorTop = 13;
                    Console.CursorLeft = (Console.WindowWidth - "Live session.".Length) / 2;
                    Console.WriteLine("Live session.");
                    Console.CursorTop = 15;
                    Console.CursorLeft = 0;
                    Console.WriteLine($"Initiating live session in {10 - seconds_passed} seconds...");
                    while(SystemInfo.Time.Second == seconds_start)
                    {
                    }
                    seconds_passed += 1;
                    seconds_start = SystemInfo.Time.Second;
                }
                Console.BackgroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.CursorTop = 13;
                Kernel.CenterWrite("Log In");
                Console.CursorTop = 15;
                Console.WriteLine("Username:");
                var usr = GetUser(Console.ReadLine());
                if (usr == null)
                {
                    Curse.ShowMessagebox("User not found.", "The username you entered was not found in the database.");
                    LoginScreen();
                    return;
                }
                Console.WriteLine("Password:");
                if (Authenticate(usr, Console.ReadLine()) == false)
                {
                    Curse.ShowMessagebox("Invalid password.", "The password you entered was invalid.");
                    LoginScreen();
                    return;
                }
                current_user = usr;

            }
        }

        public static User GetUser(string name)
        {
            if (is_livesession)
                return livesession_user;

            foreach(var user in read_userlist())
            {
                if (user.Name == name)
                    return user;
            }

            return null;
        }

        private static User[] read_userlist()
        {
            List<User> users = new List<User>();
            users.Add(system_user);
            if (File.Exists(@"0:\System\user.db"))
            {
                string[] users_unserialized = File.ReadAllLines(@"0:\System\user.db");
                foreach(string userdata in users_unserialized)
                {
                    string[] data = userdata.Split('\t');
                    User usr = new User();
                    usr.Name = data[0];
                    switch(data[1])
                    {
                        case "USER":
                            usr.PermissionLevel = 2;
                            break;
                        case "ADMIN":
                            usr.PermissionLevel = 3;
                            break;
                        default:
                            usr.PermissionLevel = 1;
                            break;
                    }
                    usr.UserDirectory = data[2];
                    users.Add(usr);
                }
            }
            User[] user_list = new User[users.Count];
            for(int i = 0; i < user_list.Length; i++)
            {
                user_list[i] = users[i];
            }
            return user_list;
        }
    }

    public class Kernel : Sys.Kernel
    {
        string env_vars = "AUTHOR:Michael VanOverbeek";

        string current_directory = "0:\\";

        public static void CenterWrite(string text)
        {
            Console.CursorLeft = (Console.WindowWidth - text.Length) / 2;
            Console.WriteLine(text);
        }

        const string kernel_version = "0.0.2";
        const string kernel_flavour = "Ladouceur";
        public const string memphis_logo = @"
  _     _                                   _         _
 |  \  / |   ___                           | |       |_|   ____
 |   \/  |  /    \   _________    _____    | |____    _   /  __|
 | |\_/| | | ( )  | |  _   _  |  |  _  \   |  __  |  | |  | |__
 | |   | | | ____/  | | | | | |  | |_|  |  | |  | |  | |  _\___ \
 |_|   |_|  \____/  |_| |_| |_|  |  ___/   |_|  |_|  |_| |_____/
                                 | |
 Powered by the C# Open Source   |_| Managed Operating System.
____________________________________________________________________
";
        protected override void BeforeRun()
        {
            KernelUtils.Init(this);
            Curse.kern = this;
            env_vars += ";VERSION:" + kernel_version + ";KERNEL:" + kernel_flavour;
            FS = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(FS);
            FS.Initialize();
            Console.WriteLine("Scanning filesystems...");
            if(!Directory.Exists(@"0:\"))
            {
                Curse.ShowMessagebox("FAT Driver", "Cosmos could not find a valid FAT filesystem on 0:\\. Some Memphis applications may not function.");
            }
            UserManager.LoginScreen();
            Console.Clear();
            Console.WriteLine(memphis_logo);
            Console.WriteLine("Welcome to Memphis.");
            Console.WriteLine($"Version: {GetVar("VERSION")}\t\tKernel flavour: {GetVar("KERNEL")}");
            Console.WriteLine($@"System information:
 - FAT partition count: {Sys.FileSystem.VFS.VFSManager.GetVolumes().Count}
 - Current user: {UserManager.CurrentUser.Name}
 - Permission level: {UserManager.CurrentUser.PermissionLevel}
 - User Directory: {UserManager.CurrentUser.UserDirectory}
 - Current date and time is: {SystemInfo.Time.ToString()}");
            

        }

        public string GetVar(string input)
        {
            string[] evars = split_str(env_vars, ";");
            foreach (string kv in evars)
            {
                string[] var = split_str(kv, ":");
                if (var[0] == input)
                {
                    return var[1];
                }
            }
            return "";
        }

        bool running = true;
        public Cosmos.System.FileSystem.CosmosVFS FS = null;
        protected override void Run()
        {
            while (running)
            {
                try
                {
                    if (TUI.Utils.Windows.Count > 0)
                    {
                        var kinf = Console.ReadKey(true);
                        if (kinf.Key == ConsoleKey.Tab)
                        {
                            TUI.Utils.Windows[TUI.Utils.SelectedWindow].UnSelect();
                            if (TUI.Utils.SelectedWindow < TUI.Utils.Windows.Count - 1)
                            {
                                TUI.Utils.SelectedWindow += 1;
                            }
                            else
                            {
                                TUI.Utils.SelectedWindow = 0;
                            }
                            TUI.Utils.Windows[TUI.Utils.SelectedWindow].Select();
                        }
                        else
                        {
                            TUI.Utils.Windows[TUI.Utils.SelectedWindow].KeyDown(kinf);
                        }
                    }
                    else
                    {
                        Console.Write(current_directory + $" [{UserManager.CurrentUser.Name}]> ");
                        string input = Console.ReadLine();
                        InterpretCMD(input);
                    }
                }
                catch(Exception ex)
                {
                    StopKernel(ex);
                }
            }
        }


        public const string sad_monitor = @"
 ____________
|            |      ______________________________________________
|  ()    ()  |     /                                              \
|   ______   | ---|  Looks like an error occurred... Now I'm sad.  |
| /        \ |     \______________________________________________/
|____________|"; //Memphis is sad because it encountered an error. It's gonna go hide in a corner. ):

        public void StopKernel(Exception ex)
        {
            //PlayErrorSound(); Dang. This doesn't seem to work...
            running = false;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Clear();
            Console.WriteLine(sad_monitor);
            string ex_message = ex.Message;
            string inner_message = "<none>";
            if (ex.InnerException != null)
                inner_message = ex.InnerException.Message;
            Console.WriteLine($@"Error message: {ex_message}
Inner exception message: {inner_message}");
            Console.WriteLine(memphis_logo);
            Console.WriteLine("Press any key to reboot.");
            try
            {
                Console.ReadKey();
            }
            catch
            {

            }
            Sys.Power.Reboot();
        }

        public void PlayErrorSound()
        {
            for(int i = 500; i > 450; i--)
            {
                Beep(i, 100);
            }
            Beep(440, 10);
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
            else if(lower.StartsWith("mousetest"))
            {
                var m = new Mouse();
            }
            else if(lower.StartsWith("conf-gen"))
            {
                StartApplicationLoop(new Apps.ConfigurationGenerator(), new[] { "" });
            }
            else if(lower.StartsWith("win_test"))
            {
                var w = new TUI.BlankWindow("Jonathan Ladouceur", 22, 10, 2, 2);
                Console.CursorLeft = 0;
                Console.CursorTop = 0;
                Console.BackgroundColor = ConsoleColor.Black;
                var b1 = new TUI.Button("He's awesome.", 2, 2, 10, 1, w);
                var b2 = new TUI.Button("NEIN", 2, 4, 10, 1, w);
                var w2 = new TUI.BlankWindow("Another Window", 32, 20, w.X + w.Width + 3, w.Y);

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
                Console.WriteLine(GetVar(input.Remove(0, 1)));
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
