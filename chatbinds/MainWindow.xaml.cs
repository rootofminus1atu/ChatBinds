using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

namespace chatbinds
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<MockGameChatKey> gcks = new List<MockGameChatKey>()
        {
            new MockGameChatKey { Name = "Minecraft", ChatKey = "T" },
            new MockGameChatKey { Name = "Terraria", ChatKey = "ENTER" }
        };
        public MainWindow()
        {
            InitializeComponent();
            GameChatKeys.ItemsSource = gcks;
        }

        private void CloseIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void GameChatKeysRemove_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine("h");
            if (sender is DataGridCell cell)
            {
                // Check if the DataContext of the cell is a MockGameChatKey
                if (cell.DataContext is MockGameChatKey selectedGameChatKey)
                {
                    // Print the data item or perform any other action
                    MessageBox.Show($"Remove clicked for {selectedGameChatKey.Name}");
                }
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("h");
            if (sender is Button button)
            {
                // Check if the DataContext of the cell is a MockGameChatKey
                if (button.DataContext is MockGameChatKey selectedGameChatKey)
                {
                    // Print the data item or perform any other action
                    MessageBox.Show($"Remove clicked for {selectedGameChatKey.Name}");
                }
            }
        }


        private void ThingsToSay_Add_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GameChatKeys_Add_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
