using SharpOS.SystemRing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpOS
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
            List<string> message_lines = new List<string>();
            message_lines.Add("+------[ " + title + " ]------+");
            string t = message_lines[0];
            int h_space = t.Length - 3;
            message_lines.Add("|" + Repeat(" ", h_space) + "|");
            int h_pad = h_space - 2;
            foreach (string c in SplitChunks(text, h_pad))
            {
                message_lines.Add("| " + c + Repeat(" ", h_pad - c.Length) + " |");
            }
            message_lines.Add("|" + Repeat(" ", h_space) + "|");
            string button = "[enter:ok] ";
            int blength = button.Length;
            int bpad = (h_space - blength) - 1;
            message_lines.Add("|" + Repeat(" ", bpad) + button + " |");
            message_lines.Add("+" + Repeat("-", h_space) + "+");
            DrawCenter(message_lines.ToArray());
            var k = Console.ReadKey();
            if (k.Key == ConsoleKey.Enter)
            {
                Console.CursorLeft = 0;
                Console.CursorTop = 0;
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
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
