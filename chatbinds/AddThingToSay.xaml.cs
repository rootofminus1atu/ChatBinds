using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace chatbinds
{
    /// <summary>
    /// Interaction logic for AddThingToSay.xaml
    /// </summary>
    public partial class AddThingToSay : Window
    {
        private readonly Db db;
        private string hotKey = null;
        private string text = null;
        public AddThingToSay(Db db)
        {
            this.db = db;
            InitializeComponent();
        }
        private void ThingToSayTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            text = ThingToSayTextBox.Text;
        }

        private void HotKeyButton_Click(object sender, RoutedEventArgs e)
        {
            this.PreviewKeyDown += ListenForKeyEvent;
            HotKeyButton.Content = "Press a key...";
            HotKeyButton.IsEnabled = false;
        }

        private void ListenForKeyEvent(object sender, KeyEventArgs e)
        {
            if (e is KeyEventArgs keyArgs)
            {
                Trace.WriteLine($"Key pressed: {keyArgs.Key}");
                HotKeyButton.Content = $"{keyArgs.Key}";
                hotKey = $"{keyArgs.Key}";
            }

            this.PreviewKeyDown -= ListenForKeyEvent;
            HotKeyButton.IsEnabled = true;
        }

        private class CatFactResponse
        {
            [JsonPropertyName("fact")]
            public string Fact { get; set; }
            [JsonPropertyName("length")]
            public int Length { get; set; }
        }

        private async void CatFactButton_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("https://catfact.ninja/fact?max_length=100");

                    if (response.IsSuccessStatusCode)
                    {
                        using (var responseStream = await response.Content.ReadAsStreamAsync())
                        {
                            var factObject = await JsonSerializer.DeserializeAsync<CatFactResponse>(responseStream);

                            var addThingToSayWindow = new CopyToClipboardMessageBox(factObject.Fact);
                            addThingToSayWindow.ShowDialog();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Failed to retrieve cat fact. Status code: {response.StatusCode}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (hotKey is null || string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Please input both a thing to say and a hotkey.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var existingThingToSay = db.GameChatKeys.Find(hotKey);
            if (existingThingToSay != null)
            {
                MessageBox.Show($"'{hotKey}' already in use.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newThingToSay = new ThingToSay
            {
                Text = text,
                HotKey = hotKey,
            };

            db.ThingsToSay.Add(newThingToSay);
            db.SaveChanges();

            Close();
        }
    }
}
