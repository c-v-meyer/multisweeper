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

namespace Multisweeper
{
    /// <summary>
    /// Interaktionslogik für JoinPartyPopup.xaml
    /// </summary>
    public partial class JoinPartyPopup : Window
    {
        private Action<bool, string> callback;
        public JoinPartyPopup(Action<bool, string> callback)
        {
            this.callback = callback;
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            callback(true, "");
            Close();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            callback(false, ip.Text);
            Close();
        }
    }
}
