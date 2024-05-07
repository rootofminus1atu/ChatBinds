using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Drawing;
using System.Drawing.Imaging;

namespace chatbinds
{
    /// <summary>
    /// Interaction logic for CopyToClipboardMessageBox.xaml
    /// </summary>
    public partial class CopyToClipboardMessageBox : Window
    {
        public CopyToClipboardMessageBox(string message)
        {
            InitializeComponent();
            messageTextBlock.Text = message;
            iconImage.Source = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Information.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(messageTextBlock.Text);
            Close();
        }
    }
}
