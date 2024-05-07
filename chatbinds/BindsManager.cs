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
    /// <summary>
    /// Represents a manager, who manages both the keyboard hook for listening, and the input simulator for using the user's keyboard on their behalf.
    /// Comes with methods to Start/Stop it and to Re-register the binds, which will keep them up-to-date.
    /// </summary>
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

        /// <summary>
        /// Starts the BindsManager. Does NOT automatically re-register any of the binds.
        /// <see cref="ReRegisterThingsToSay(IEnumerable{ThingToSay})"/> has to be called explicitly.
        /// </summary>
        public void Start()
        {
            if (!IsRunning)
            {
                keyboardHookManager.Start();
                IsRunning = true;
            }
        }

        /// <summary>
        /// Stops the BindsManager.
        /// </summary>
        public void Stop()
        {
            if (IsRunning)
            {
                keyboardHookManager.Stop();
                IsRunning = false;
            }
        }

        /// <summary>
        /// Refreshes all the binds, so that they're up-to-date with the newest data, which means being up-to-date both with GamChatKeys and ThingsToSay.
        /// </summary>
        /// <param name="thingsToSay"></param>
        public void ReRegisterBinds(IEnumerable<ThingToSay> thingsToSay)
        {
            // clearning the previous binds
            keyboardHookManager.UnregisterAll();
            foreach (var thing in thingsToSay)
            {
                keyboardHookManager.RegisterHotkey((int)GetVirtualKeyCode(thing.HotKey), () =>
                {
                    // getting the correct chat key for the currently running game
                    var current = FindCurrentGameChatKey();
                    if (current != null)
                    {
                        inputSimulator.Keyboard.KeyPress(GetVirtualKeyCode(current.ChatKey));
                        // the sleeps are needed for some games, since they might not have enough time to react to events happening faster
                        Thread.Sleep(10);
                        inputSimulator.Keyboard.KeyPress(VirtualKeyCode.BACK);
                        Thread.Sleep(10);
                        inputSimulator.Keyboard.TextEntry(thing.Text);
                        Thread.Sleep(10);
                        inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                    }
                });
            }
        }

        /// <summary>
        /// Turns a string into a VirtualKeyCode object, which can be more easily dealt with. Currently it might break, however it can be extended to work with other keys too. A custom implementation that's more robust and general could be provided in a future release.
        /// </summary>
        /// <param name="keyName">The string to be turned into a VirtualKeyCode</param>
        /// <returns>The associated VirtualKeyCode</returns>
        private VirtualKeyCode GetVirtualKeyCode(string keyName)
        {
            // some special cases
            // there very likely has to be more 
            if (keyName == "OemQuestion")
            {
                return (VirtualKeyCode)0xBF;
            }

            return (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), keyName, ignoreCase: true);
        }

        /// <summary>
        /// Finds the current GameChatKey object (can be null). For example if the user is playing Terraria and terraria was saved to the db, an object with it and its corresponding chat key would be retriedved. If not founds returns null.
        /// </summary>
        /// <returns>The current GameChatKey object</returns>
        private GameChatKey FindCurrentGameChatKey()
        {
            var currentWindow = GetActiveProcessName();
            return db.GameChatKeys.Find(currentWindow);
        }

        // below there are some special imports to detect the currently running process

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        /// <summary>
        /// Retrieves the currently running process in the user's foreground. For example, if the user is running Terraria and Minecraft, but Terraria is in focus (while Minecraft is hidden away in the backgroun), the process name for Terraria would be retrieved.
        /// </summary>
        /// <returns>the currently running process in the user's foregroun.</returns>
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
