using Memphis.SystemRing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memphis
{
    class Curse
    {
        public static Kernel kern = null;

        
        public static string GetText(string prompt, int space)
        {
            List<char> potential_string = new List<char>();
            int char_offset = 0;
            int x = 0;
            bool done = false;
            string s = "";
            string[] text_to_draw = { prompt + ": [" + Repeat(" ", space) + "]" };
            DrawCenter(text_to_draw);
            int cornerx = ((Console.WindowWidth - text_to_draw[0].Length) / 2) + text_to_draw[0].IndexOf("[");
            while (done == false)
            {
                Console.CursorLeft = cornerx;
                if(potential_string.Count < space)
                {
                    foreach(var c in potential_string)
                    {
                        Console.Write(c);
                    }
                }
                else
                {
                    for(int i = char_offset; i < space + char_offset; i++)
                    {
                        try
                        {
                            Console.Write(potential_string[i]);
                        }
                        catch
                        {

                        }
                    }
                }
                Console.CursorLeft = cornerx + x;
                var inf = Console.ReadKey();
                switch(inf.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if(x > 0)
                        {
                            x -= 1;
                        }
                        else
                        {
                            if(char_offset > 0)
                            {
                                char_offset -= 1;
                            }
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if(x < space)
                        {
                            if (x < potential_string.Count)
                            {
                                x += 1;
                            }
                        }
                        else
                        {
                            if(char_offset < potential_string.Count)
                            {
                                char_offset += 1;
                            }
                        }
                        break;
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.Tab:
                    case ConsoleKey.Escape:

                        break;
                    case ConsoleKey.Enter:
                        s = "";
                        foreach(var c in potential_string)
                        {
                            s += c;
                        }
                        done = true;
                        break;
                    default:
                        try
                        {
                            potential_string.Insert(x, inf.KeyChar);
                            if (x < space)
                            {
                                if (x < potential_string.Count)
                                {
                                    x += 1;
                                }
                            }
                            else
                            {
                                if (char_offset < potential_string.Count)
                                {
                                    char_offset += 1;
                                }
                            }

                        }
                        catch
                        {

                        }
                            break;
                }
            }
            return s;
        }

        public static void RunCommand(string text)
        {
            //Pipe command to kernel
            kern.InterpretCMD(text);
        }

        public static void ShowMessagebox(string title, string text)
        {
            int splitWidth = 25;
            if(text.Length < splitWidth)
            {
                splitWidth = text.Length;
            }
            if(title.Length > splitWidth)
            {
                splitWidth = title.Length;
            }
            var lines = new List<string>();
            if(splitWidth > text.Length)
            {
                lines.Add(text);
            }
            else
            {
                lines = TUI.Utils.split_string(splitWidth, text);
            }
            foreach(var line in lines)
            {
                if(text.Contains(line))
                {
                    text = text.Replace(line, "");
                }
            }
            if(text.Length > 0)
            {
                lines.Add(text);
            }
            int h = lines.Count + 4;
            int w = 0;
            foreach(var line in lines)
            {
                if(line.Length + 4 > w)
                {
                    w = line.Length + 4;
                }
            }
            int x = (Console.WindowWidth - w) / 2;
            int y = (Console.WindowHeight - h) / 2;
            TUI.Utils.ClearArea(x + 1, y + 1, w, h, ConsoleColor.Black);
            TUI.Utils.ClearArea(x, y, w, h, ConsoleColor.Green);
            TUI.Utils.ClearArea(x, y, w, 1, ConsoleColor.White);
            TUI.Utils.Write(x + 1, y, title, ConsoleColor.White, ConsoleColor.Black);
            for(int i = 0; i < lines.Count - 1; i++)
            {
                TUI.Utils.Write(x + 2, (y + 2) + i, lines[i], ConsoleColor.Green, ConsoleColor.White);
            }
            int xw = x + w;
            int yh = y + h;
            TUI.Utils.Write(xw - 6, yh - 2, "<OK>", TUI.Utils.COL_BUTTON_SELECTED, TUI.Utils.COL_BUTTON_TEXT);
            bool stuck = true;
            while (stuck)
            {
                var kinf = Console.ReadKey();
                if (kinf.Key == ConsoleKey.Enter)
                {
                    stuck = false;
                    Console.Clear();
                }
                else
                {

                }
            }
        }

        public static void DrawCenter(string[] contents)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.Red;
            int cornerx = (Console.WindowWidth - contents[0].Length) / 2;
            int cornery = (Console.WindowHeight - contents.Length) / 2;
            for (int i = 0; i < contents.Length; i++)
            {
                Console.CursorLeft = cornerx;
                Console.CursorTop = cornery + i;
                Console.Write(contents[i]);
            }


        }

        public static string Repeat(string text, int length)
        {
            string t = text;
            while (text.Length <= length)
            {
                text += t;
            }
            return text;
        }

        public static string[] SplitChunks(string orig, int size)
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

    }
}
