using System.Runtime.InteropServices;

namespace MousEye
{
    public static class MouseManipulation
    {
        [DllImport("User32.dll")]
        public static extern bool SetCursorPos(int x, int y);
    }
}