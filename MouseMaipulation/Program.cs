using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MouseMaipulation
{
    internal class Program
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy,
                              int dwData, int dwExtraInfo);

        [Flags]
        private enum MouseEventFlags
        {
            Leftdown = 0x00000002,
            Leftup = 0x00000004,
            Middledown = 0x00000020,
            Middleup = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            Rightdown = 0x00000008,
            Rightup = 0x00000010
        }

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        private static void Main(string[] args)
        {
            Console.WriteLine(Cursor.Position.ToString());
            Console.ReadKey();

            SetCursorPos(1850, 1000);

            Console.WriteLine(Cursor.Position.ToString());
            Console.ReadKey();

            Console.WriteLine(Cursor.Position.ToString());
            mouse_event((int)MouseEventFlags.Leftdown, 1850, 1000, 0, 0);
            mouse_event((int)MouseEventFlags.Leftup, 1850, 1000, 0, 0);
            mouse_event((int)MouseEventFlags.Leftdown, 1850, 1000, 0, 0);
            mouse_event((int)MouseEventFlags.Leftup, 1850, 1000, 0, 0);


            Console.ReadKey();
        }
    }
}