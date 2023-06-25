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
using System.Windows.Controls.Primitives;
using System.Threading;

namespace Multisweeper
{
    /// <summary>
    /// Interaktionslogik für StartupWindow.xaml
    /// </summary>
    public partial class StartupWindow : Window
    {
        public StartupWindow()
        {
            InitializeComponent();
        }

        private void JoinPartyButton_Click(object sender, RoutedEventArgs e)
        {
            joinPartyButton.IsEnabled = false;
            createPartyButton.IsEnabled = false;
            JoinPartyPopup popup = new JoinPartyPopup(JoinPartyCallback);
            popup.Show();
        }

        public void JoinPartyCallback(bool cancel, string ip)
        {
            joinPartyButton.IsEnabled = true;
            createPartyButton.IsEnabled = true;
            if (!cancel)
            {
                System.Diagnostics.Trace.WriteLine(ip);
                //TODO
            }
        }

        private void CreatePartyButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(true);
            mainWindow.Show();
            Close();
        }
    }
}
