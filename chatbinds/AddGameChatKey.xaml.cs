using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace chatbinds
{
    /// <summary>
    /// Interaction logic for AddGameChatKey.xaml
    /// </summary>
    public partial class AddGameChatKey : Window
    {
        private readonly Db db;
        private string chatKey = null;
        private string gameName = null;

        public AddGameChatKey(Db db)
        {
            this.db = db;
            InitializeComponent();
        }

        private void ChatKeyButton_Click(object sender, RoutedEventArgs e)
        {
            this.PreviewKeyDown += ListenForKeyEvent;
            ChatKeyButton.Content = "Press a key...";
            ChatKeyButton.IsEnabled = false;
        }

        private void ListenForKeyEvent(object sender, KeyEventArgs e)
        {
            if (e is KeyEventArgs keyArgs)
            {
                Trace.WriteLine($"Key pressed: {keyArgs.Key}");
                ChatKeyButton.Content = $"{keyArgs.Key}";
                chatKey = $"{keyArgs.Key}";
            }

            this.PreviewKeyDown -= ListenForKeyEvent;
            ChatKeyButton.IsEnabled = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (gameName is null ||  chatKey is null)
            {
                MessageBox.Show("Please select both a game and a chat key.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var existingGameChatKey = db.GameChatKeys.Find(gameName);
            if (existingGameChatKey != null)
            {
                MessageBox.Show($"A chat key for '{gameName}' already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newGameChatKey = new GameChatKey
            {
                Name = gameName,
                ChatKey = chatKey
            };

            db.GameChatKeys.Add(newGameChatKey);
            db.SaveChanges();

            Close();
        }

        private void GameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GameComboBox.SelectedItem != null)
            {
                gameName = GameComboBox.SelectedItem.ToString();
            }
        }

        private void GameComboBox_DropDownOpened(object sender, EventArgs e)
        {
            var ps = GetProcesses().Select(p => p.ProcessName);
            Trace.WriteLine(ps.Stringify());
            GameComboBox.ItemsSource = new List<string>(ps);
        }

        private static IEnumerable<Process> GetProcesses()
        {
            return Process.GetProcesses()
                .Where(p => p.MainWindowHandle != IntPtr.Zero && p.ProcessName != "explorer");
        }
    }
}
