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
        private readonly BindsManager bindsManager;

        public MainWindow()
        {
            bindsManager = new BindsManager(db);
            InitializeComponent();
            LoadGameChatKeys();
            LoadThingsToSay();

            bindsManager.Start();
            UpdateListeningStatus();
        }

        // refreshers below

        private void LoadGameChatKeys()
        {
            GameChatKeys.ItemsSource = db.GameChatKeys.ToList();
        }

        private void LoadThingsToSay()
        {
            var thingsToSay = db.ThingsToSay.ToList();

            bindsManager.ReRegisterBinds(thingsToSay);

            ThingsToSay.ItemsSource = thingsToSay;
        }


        // handling the GameChatKeys events (adding and removing)

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
                    db.GameChatKeys.Remove(selectedGameChatKey);
                    db.SaveChanges();
                    LoadGameChatKeys();
                }
            }
        }


        // handling the THingsToSay events (adding and removing)

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
                    db.ThingsToSay.Remove(selectedThingToSay);
                    db.SaveChanges();
                    LoadThingsToSay();
                }
            }
        }


        // handling the bindsManager status (listening or not listening)

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (bindsManager.IsRunning == true)
                bindsManager.Stop();
            else
                bindsManager.Start();

            UpdateListeningStatus();
        }

        private void UpdateListeningStatus()
        {
            ListeningStatusTextBlock.Foreground = bindsManager.IsRunning ? Brushes.Green : Brushes.Red;
            ListeningStatusTextBlock.Text = bindsManager.IsRunning ? "Listening!" : "Not listening";
        }
    }
}
