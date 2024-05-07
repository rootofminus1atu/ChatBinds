using NonInvasiveKeyboardHookLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsInput;
using WindowsInput.Native;

namespace chatbinds
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Db db = new Db();
        private KeyboardHookManager keyboardHookManager = new KeyboardHookManager();
        private bool keyboardHookManagerRunning = false;
        InputSimulator inputSimulator = new InputSimulator();

        public MainWindow()
        {
            InitializeComponent();
            LoadGameChatKeys();
            LoadThingsToSay();

            StartKeyboardHook();

        }

        private VirtualKeyCode GetVirtualKeyCode(string keyName)
        {
            // some special cases
            if (keyName == "OemQuestion")
            {
                Trace.WriteLine($"oem question lol {0xBF} -> {(VirtualKeyCode)0xBF}");
                return (VirtualKeyCode)0xBF;
            }
            
            return (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), keyName, ignoreCase: true);
        }

        private void LoadGameChatKeys()
        {
            GameChatKeys.ItemsSource = db.GameChatKeys.ToList();
        }

        private void LoadThingsToSay()
        {
            var thingsToSay = db.ThingsToSay.ToList();

            ReRegisterThingsToSay(thingsToSay);

            ThingsToSay.ItemsSource = thingsToSay;
        }

        private void ReRegisterThingsToSay(IEnumerable<ThingToSay> thingsToSay)
        {
            keyboardHookManager.UnregisterAll();
            foreach (var thing in thingsToSay)
            {
                Trace.WriteLine($"Registering {thing.Text} for hotkey {thing.HotKey} (vkc {(int)GetVirtualKeyCode(thing.HotKey)})");
                keyboardHookManager.RegisterHotkey((int)GetVirtualKeyCode(thing.HotKey), () =>
                {
                    //Trace.WriteLine($"space pls");
                    //inputSimulator.Keyboard.KeyPress(VirtualKeyCode.SPACE);
                    //Trace.WriteLine($"space pls pressed");

                    var current = FindCurrentGameChatKey();
                    if (current is null)
                    {
                        return;
                    }
                    Trace.WriteLine($"current not null");
                    inputSimulator.Keyboard.KeyPress(GetVirtualKeyCode(current.ChatKey));
                    Thread.Sleep(500);
                    Trace.WriteLine($"pressed chat key {current.ChatKey}");
                    inputSimulator.Keyboard.TextEntry(thing.Text);
                    Thread.Sleep(500);
                    Trace.WriteLine($"sending text");
                    inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                    Trace.WriteLine($"pressing enter");
                });
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder sb = new StringBuilder(nChars);
            IntPtr hWnd = GetForegroundWindow();

            if (hWnd != IntPtr.Zero)
            {
                GetWindowText(hWnd, sb, nChars);
            }

            return sb.ToString();
        }
        private string GetActiveProcessName()
        {
            IntPtr hwnd = GetForegroundWindow();
            uint processId;
            GetWindowThreadProcessId(hwnd, out processId);
            Process process = Process.GetProcessById((int)processId);
            return process.ProcessName;
        }

        private GameChatKey FindCurrentGameChatKey()
        {
            var currentWindow = GetActiveProcessName();
            var res = db.GameChatKeys.Find(currentWindow);
            Trace.WriteLine($"current window {res?.Name} with ck {res?.ChatKey}");

            return res;
        }

        private void GameChatKeys_Add_Click(object sender, RoutedEventArgs e)
        {
            var addGameChatKeyWindow = new AddGameChatKey(db);
            addGameChatKeyWindow.ShowDialog();
            LoadGameChatKeys();
        }

        private void GameChatKeys_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.DataContext is GameChatKey selectedGameChatKey)
                {
                    MessageBox.Show($"Remove clicked for {selectedGameChatKey.Name}");
                    db.GameChatKeys.Remove(selectedGameChatKey);
                    db.SaveChanges();
                    LoadGameChatKeys();
                }
            }
        }

        private void ThingsToSay_Add_Click(object sender, RoutedEventArgs e)
        {
            var addThingToSayWindow = new AddThingToSay(db);
            addThingToSayWindow.ShowDialog();
            LoadThingsToSay();
        }
        private void ThingsToSay_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.DataContext is ThingToSay selectedThingToSay)
                {
                    MessageBox.Show($"Remove clicked for {selectedThingToSay.Text}");
                    db.ThingsToSay.Remove(selectedThingToSay);
                    db.SaveChanges();
                    LoadThingsToSay();
                }
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (keyboardHookManagerRunning == true)
            {
                StopKeyboardHook();
            } 
            else
            {
                StartKeyboardHook();
            }
        }

        private void StopKeyboardHook()
        {
            keyboardHookManager.Stop();
            keyboardHookManagerRunning = false;
            ListeningStatusTextBlock.Foreground = Brushes.Red;
            ListeningStatusTextBlock.Text = "Not listening";
        }

        private void StartKeyboardHook()
        {
            keyboardHookManager.Start();
            keyboardHookManagerRunning = true;
            ListeningStatusTextBlock.Foreground = Brushes.Green;
            ListeningStatusTextBlock.Text = "Listening!";
        }
    }
}
