using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Memphis;
using System.IO;

namespace Memphis.Apps
{
    class HelpViewer : IApplication
    {
        string help_string = "";

        public List<string> help_keys = new List<string>();
        public List<string> help_vals = new List<string>();
        bool viewing = false;
        int help_doc = 0;
        string ex = null;

        public override void Start(string[] args)
        {
            if (!File.Exists(args[0]))
                ex = "Couldn't find help database!";

            if(ex == null)
            {
                r = true;
            } 

            help_string = File.ReadAllText(args[0]); //get help string.

            ParseHelp();

            if(args[1] != null)
            {
                bool show_error = true;
                for(int i = 0; i < help_keys.Count; i++)
                {
                    if(help_keys[i].ToLower() == args[1].ToLower())
                    {
                        viewing = true;
                        show_error = false;
                        help_doc = i;
                    }
                }
                if (show_error)
                    Curse.ShowMessagebox("Help Viewer - Topic not found.", "Help Viewer could not find the topic specified in the command argument.");
            }
        }

        public void ParseHelp()
        {
            help_keys = new List<string>();
            help_vals = new List<string>();

            string[] kvs = help_string.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            foreach(var kv in kvs)
            {
                string[] val = kv.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                help_keys.Add(val[0]);
                help_vals.Add(val[1]);
            }
        }

        bool r = false;

        public override bool Running {
            get
            {
                return r;
            }
            set
            {
                r = value;
            }
        }

        int page = 0;
        int items_per_page = 10;

        public override void MainLoop()
        {
            Console.Clear();
            Console.CursorLeft = 1;
            Console.CursorTop = 1;
            if (viewing == false)
            {
                Console.Write(@"Memphis Help Viewer
==========================

Topics:
"); //Verbatim String Master Race.
                var lst = GetRange(page * items_per_page, items_per_page);
                for (int i = 0; i < lst.Count; i++)
                {
                    Console.CursorLeft = 2;
                    Console.WriteLine(i + ": " + help_keys[i]);
                }
                Console.WriteLine(@"
 To quit, type 'q'. For next page, type '>'. For previous page, type '<'. Type a name to see more info.");
                Console.CursorLeft = 1;
                Console.Write("Your choice: ");
                string req = Console.ReadLine();
                switch (req)
                {
                    case ">":
                    case "next":
                    case "n":
                        if (page * items_per_page < help_keys.Count)
                        {
                            page += 1;
                        }
                        break;
                    case "<":
                    case "previous":
                    case "p":
                        if (page > 0)
                        {
                            page -= 1;
                        }
                        break;
                    case "exit":
                    case "quit":
                    case "e":
                    case "q":
                        r = false;
                        break;
                    default:
                        try
                        {
                            for (int i = 0; i < help_keys.Count; i++)
                            {
                                if (help_keys[i].ToLower() == req.ToLower())
                                {
                                    viewing = true;
                                    help_doc = i;
                                }
                            }
                        }
                        catch
                        {

                        }
                        break;
                }
            }
            else
            {
                Console.Write(@"Memphis Help Viewer
==========================

" + help_keys[help_doc] + @":
---------

" + help_vals[help_doc] + @"

 <Hit Enter to return to help screen.>
"); //Again - Verbatim String Master Race.
                Console.ReadLine();
                viewing = false;
            }
        }

        public override void End()
        {
            Console.Clear();
            if (ex != null)
            {
                Console.WriteLine(ex);
            }
            else
            {
                Console.WriteLine("Hope we helped.");
            }
        }

        public List<string> GetRange(int start, int count)
        {
            if (start > help_keys.Count - 1)
                throw new ArgumentException("Start index goes beyond count of list.");

            if (count < 1)
                throw new ArgumentException("Count must be above 0");

            if (help_keys.Count <= 0)
                throw new ArgumentException("List count is 0.");

            while (start + count > help_keys.Count)
            {
                count -= 1;
            }

            List<string> rlist = new List<string>();
            for(int i = start; i < start + count; i++)
            {
                rlist.Add(help_keys[i]);
            }
            return rlist;
        }

        public int IndexOf(string value)
        {
            int index = -1;
            for(int i = 0; i < help_keys.Count; i++)
            {
                if(help_keys[i] == value)
                {
                    index = i;
                }
            }
            if(index >= 0)
            {
                return index;
            }
            else
            {
                throw new Exception("Value not found in list!");
            }
        }
    }
}
