using Cosmos.HAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpOS.HardwareRing;

namespace SharpOS.SystemRing
{
    public class Keyboard
    {
        public static Cosmos.HAL.Keyboard kbInstance = null;

        public static void Start()
        {
            Console.WriteLine("Starting keyboard on ring SYSTEM...");
            kbInstance = new Cosmos.HAL.PS2Keyboard();
        }

        public static KeyInfo ReadKey()
        {
            var ks = Global.Keyboard.ReadKey();
            var kinf = new KeyInfo();
            kinf.Char = ks.KeyChar;
            switch (ks.Key)
            {
                case ConsoleKeyEx.Tab:
                    kinf.Key = "tab";
                    break;
                case ConsoleKeyEx.LeftArrow:
                    kinf.Key = "leftarrow";
                    break;
                case ConsoleKeyEx.RightArrow:
                    kinf.Key = "rightarrow";
                    break;
                case ConsoleKeyEx.UpArrow:
                    kinf.Key = "uparrow";
                    break;
                case ConsoleKeyEx.DownArrow:
                    kinf.Key = "downarrow";
                    break;
                case ConsoleKeyEx.Enter:
                    kinf.Key = "enter";
                    break;
                case ConsoleKeyEx.Backspace:
                    kinf.Key = "backspace";
                    break;
            }
            string test = ks.Modifiers.ToString();
            Console.Write(ks.KeyChar.ToString());
            return kinf;
        }
    }

    public class KeyInfo
    {
        public string Key { get; set; }
        public char Char { get; set; }
        public bool Control { get; set; }
        public bool Alt { get; set; }
        public bool Shift { get; set; }
    }
}
