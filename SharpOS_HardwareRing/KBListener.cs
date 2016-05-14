using Cosmos.HAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpOS.HardwareRing
{
    public class KBListener
    {
        private static Cosmos.HAL.Keyboard cKeyboard = null;

        public static string ReadKey()
        {
            if (cKeyboard == null)
                cKeyboard = new Cosmos.HAL.PS2Keyboard();

            var k = cKeyboard.ReadKey();
            var Key = k.Key;
            var Control = k.Modifiers.HasFlag(ConsoleModifiers.Control);
            var Alt = k.Modifiers.HasFlag(ConsoleModifiers.Alt);
            var Shift = k.Modifiers.HasFlag(ConsoleModifiers.Shift);
            return Key + ";" + Control + ";" + Alt + ";" + "Shift";


        }

        public static event EventHandler OnKeyPress;

    }

    public abstract class HKeyInfo
    {
        public abstract ConsoleKeyEx Key { get; set; }
        public abstract bool Control { get; set; }
        public abstract bool Alt { get; set; }
        public abstract bool Shift { get; set; }
    }
}
