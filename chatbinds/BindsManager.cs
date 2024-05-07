using NonInvasiveKeyboardHookLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace chatbinds
{
    internal class BindsManager
    {
        private readonly Db db;
        private readonly KeyboardHookManager keyboardHookManager = new KeyboardHookManager();
        private readonly InputSimulator inputSimulator = new InputSimulator();
        public bool IsRunning { get; private set; } = false;

        public BindsManager(Db db)
        {
            this.db = db;
        }

        public void Start()
        {
            if (!IsRunning)
            {
                keyboardHookManager.Start();
                IsRunning = true;
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                keyboardHookManager.Stop();
                IsRunning = false;
            }
        }

        private void ReRegisterThingsToSay(IEnumerable<ThingToSay> thingsToSay)
        {
            keyboardHookManager.UnregisterAll();
            foreach (var thing in thingsToSay)
            {
                // Trace.WriteLine($"Registering {thing.Text} for hotkey {thing.HotKey} (vkc {(int)GetVirtualKeyCode(thing.HotKey)})");
                keyboardHookManager.RegisterHotkey((int)GetVirtualKeyCode(thing.HotKey), () =>
                {
                    var current = FindCurrentGameChatKey();
                    if (current != null)
                    {
                        inputSimulator.Keyboard.KeyPress(GetVirtualKeyCode(current.ChatKey));
                        Thread.Sleep(10);
                        inputSimulator.Keyboard.TextEntry(thing.Text);
                        Thread.Sleep(10);
                        inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                    }
                });
            }
        }

        private VirtualKeyCode GetVirtualKeyCode(string keyName)
        {
            // some special cases
            // there very likely has to be more 
            if (keyName == "OemQuestion")
            {
                Trace.WriteLine($"oem question lol {0xBF} -> {(VirtualKeyCode)0xBF}");
                return (VirtualKeyCode)0xBF;
            }

            return (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), keyName, ignoreCase: true);
        }




        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private string GetActiveProcessName()
        {
            IntPtr hwnd = GetForegroundWindow();
            uint processId;
            GetWindowThreadProcessId(hwnd, out processId);
            Process process = Process.GetProcessById((int)processId);
            return process.ProcessName;
        }
    }
}
