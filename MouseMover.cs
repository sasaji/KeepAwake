using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace KeepAwake
{
    class MouseMover
    {
        private volatile bool _shouldStop;

        public void Move()
        {
            while (!_shouldStop) {
                DoMove();
                Thread.Sleep(10000);
            }
        }

        public void RequestStop()
        {
            _shouldStop = true;
        }

        private void DoMove()
        {
            INPUT input = new INPUT();
            input.type = INPUT_MOUSE;
            input.mi = new MOUSEINPUT();

            input.mi.dwExtraInfo = IntPtr.Zero;
            //input.mi.dx = 0;
            //input.mi.dy = 0;
            int diff = DateTime.Now.Millisecond % 2 == 0 ? 1 : -1;
            input.mi.dx = diff;
            input.mi.dy = diff;
            input.mi.time = 0;
            input.mi.mouseData = 0;
            input.mi.dwFlags = 0x0001; // MOVE (RELATIVE)
            int cbSize = Marshal.SizeOf(typeof(INPUT));
            uint r = SendInput(1, ref input, cbSize);
        }

        #region Win32 API
        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        const int INPUT_MOUSE = 0;

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            ushort wVk;
            ushort wScan;
            uint dwFlags;
            uint time;
            IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            uint uMsg;
            ushort wParamL;
            ushort wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)] //*
            public MOUSEINPUT mi;
            [FieldOffset(4)] //*
            public KEYBDINPUT ki;
            [FieldOffset(4)] //*
            public HARDWAREINPUT hi;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            // Legacy flag, should not be used.
            // ES_USER_PRESENT   = 0x00000004,
            ES_CONTINUOUS = 0x80000000,
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
        #endregion
    }
}
