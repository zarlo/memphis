using Memphis.SystemRing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memphis.Apps
{
    class SharpPad : IApplication
    {
        public List<string> lines = null;
        public int char_offset = 0;
        public int scroll = 0;
        public string file_path = null;


        //Keep track of cursor positions after redraws
        public int xCoord = 0;
        public int yCoord = 0;

        public bool in_menu = false;
        public int selected_item = 0;

        public override void Start(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Clear();
            Curse.ShowMessagebox("SharpPad - Broken.", "SharpPad is being rewritten and is broken.");
        }

        public void LoadFromFile()
        {
            string file = File.ReadAllText(file_path);
            LoadedContents = file;
            RedrawEntireScreen = true;
            in_menu = false;
        }

        public string LoadedContents = "";

        public override bool Running { get; set; }

        public override void MainLoop()
        {
            try
            {
                if(RedrawEntireScreen)
                {
                    if(in_menu)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                    }
                    Console.Clear();
                    TUI.Utils.ClearArea(0, 0, Console.WindowWidth, 1, ConsoleColor.Gray);
                    TUI.Utils.Write(1, 0, "SharpPad", ConsoleColor.Gray, ConsoleColor.White);
                }
                if (in_menu)
                {
                    for (int i = 0; i < menu_items.Length; i++)
                    {
                        Console.CursorLeft = 2;
                        Console.CursorTop = 2 + i;
                        if (i == selected_item)
                        {
                            Console.BackgroundColor = ConsoleColor.Gray;
                            Console.Write("> " + menu_items[i]);
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write("  " + menu_items[i]);
                        }

                    }
                    var inf = Console.ReadKey();
                    switch (inf.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if (selected_item > 0)
                            {
                                selected_item -= 1;
                            }
                            break;
                        case ConsoleKey.DownArrow:
                            if (selected_item < menu_items.Length - 1)
                            {
                                selected_item += 1;
                            }
                            break;
                        case ConsoleKey.Enter:
                            PerformMenuAction();
                            break;
                        case ConsoleKey.Escape:
                            if (lines != null)
                            {
                                in_menu = false;
                                RedrawEntireScreen = true;
                            }
                            break;
                    }
                }
                else
                {

                }
            }
            catch
            {

            }
        }

        public void MoveLeft()
        {
            string line = lines[yCoord + scroll];
            if (xCoord > 0)
            {
                xCoord -= 1;
            }
            if (line.Length > Console.WindowWidth)
            {
                if (char_offset > 0)
                {
                    char_offset -= 1;
                    xCoord += 1;
                    RedrawEntireScreen = true;
                }

            }

        }

        public void MoveRight()
        {
            string line2 = lines[yCoord + scroll];
            if (xCoord < Console.WindowWidth - 1)
            {
                if (xCoord < line2.Length)
                {
                    xCoord += 1;
                }
            }
            else
            {
                if (line2.Length >= Console.WindowWidth - 1)
                {
                    if (char_offset < line2.Length - 1)
                    {
                        char_offset += 1;
                        RedrawEntireScreen = true;
                    }
                }
            }
        }

        public void MoveDown()
        {
            if (yCoord + scroll < lines.Count - 1)
            {
                if (yCoord < Console.WindowHeight - 3)
                {
                    yCoord += 1;
                }
                else
                {
                    if (scroll < lines.Count - 1)
                    {
                        scroll += 1;
                        RedrawEntireScreen = true;
                    }
                }
            }

        }

        public void MoveToEnd()
        {
            string str = lines[yCoord + scroll];
            if(str.Length > Console.WindowWidth)
            {
                while(xCoord < Console.WindowWidth)
                {
                    xCoord += 1;
                }
                while(char_offset < str.Length - Console.WindowWidth)
                {
                    char_offset += 1;
                }
            }
            else
            {
                while(xCoord < str.Length)
                {
                    xCoord += 1;
                }
            }
        }

        public void MoveUp()
        {
            if (yCoord > 0)
            {
                yCoord -= 1;
            }
            else
            {
                if (scroll > 0)
                {
                    scroll -= 1;
                    RedrawEntireScreen = true;
                }
            }

        }

        public void DrawText(List<string> lns)
        {
            //Just draw all lines on screen. It doesn't matter what the scroll is.
            for (int i = 0; i < lns.Count; i++)
            {
                Console.CursorLeft = 0;
                Console.CursorTop = 1 + i;
                string ln = lns[i];
                if (i == yCoord)
                {
                    if (ln.Length > Console.WindowWidth)
                    {
                        char[] chars = get_char_array(ln);
                        for (int x = char_offset; x < Console.WindowWidth + char_offset; x++)
                        {
                            Console.Write(chars[x]); //write all chars in range
                        }
                    }
                    else
                    {
                        foreach (char c in get_char_array(ln))
                        {
                            Console.Write(c);
                        }
                    }
                }
                else
                {
                    //No offsets this time!
                    if (ln.Length > Console.WindowWidth)
                    {
                        char[] chars = get_char_array(ln);
                        for (int x = 0; x < Console.WindowWidth; x++)
                        {
                            Console.Write(chars[x]); //write all chars in range
                        }
                    }
                    else
                    {
                        foreach (char c in get_char_array(ln))
                        {
                            Console.Write(c);
                        }
                    }
                }
            }
        }

        public bool RedrawEntireScreen = true; //way to get around immense flickering with large files

        public char[] get_char_array(string str)
        {
            List<char> chars = new List<char>();
            foreach(char c in str)
            {
                chars.Add(c);
            }
            return chars.ToArray();
        }

        public void PerformMenuAction()
        {
            switch(menu_items[selected_item])
            {
                case "New":
                    if(file_path == null)
                    {
                        lines = new List<string>();
                        lines.Add(" ");
                        xCoord = 0;
                        yCoord = 0;
                        scroll = 0;
                        char_offset = 0;
                        in_menu = false;
                        RedrawEntireScreen = true;
                    }
                    break;
                case "Load from File":
                    try
                    {
                        string path = Curse.GetText("Filename", 45);
                        if (File.Exists(path))
                        {
                            file_path = path;
                            LoadFromFile();
                        }
                        else
                        {
                            Curse.ShowMessagebox("SharpPad - File not found.", "The file \"" + path + "\" was not found on the system.");
                        }
                    }
                    catch
                    {
                        Curse.ShowMessagebox("Cosmos Kernel Error", "The Cosmos kernel failed to process the data.");
                    }
                    break;
                case "Save":
                    if (!string.IsNullOrEmpty(file_path) && File.Exists(file_path))
                    {
                        SaveFile();
                        RedrawEntireScreen = true;
                        in_menu = false;
                    }
                    else
                    {
                        selected_item += 1;
                        PerformMenuAction();
                    }
                    break;
                case "Save as":
                    try
                    {
                        string path = Curse.GetText("Filename", 45);
                        if (!File.Exists(path))
                        {
                            File.Create(path).Close();
                        }
                        file_path = path;
                        SaveFile();
                    }
                    catch
                    {
                        Curse.ShowMessagebox("Cosmos Kernel Error", "The Cosmos kernel failed to process the data.");
                    }
                    break;
                case "Help":
                    Curse.RunCommand("help sharppad");
                    break;
                case "Exit":
                    lines = null;
                    Running = false;
                    break;
            }
        }

        public string[] menu_items = { "New", "Load from File", "Save", "Save as", "Help", "Exit" };

        public override void End()
        {
           
        }

        public void SaveFile()
        {
            string[] f = lines.ToArray();
            File.WriteAllLines(file_path, f);
        }

        public List<string> get_range(int start, int count)
        {
            if (start > lines.Count - 1)
                throw new ArgumentException("Start index goes beyond count of list.");

            if (count < 1)
                throw new ArgumentException("Count must be above 0");

            if (lines.Count <= 0)
                throw new ArgumentException("List count is 0.");

            while (start + count > lines.Count)
            {
                count -= 1;
            }

            List<string> rlist = new List<string>();
            for (int i = start; i < start + count; i++)
            {
                rlist.Add(lines[i]);
            }
            return rlist;
        }

    }
}
