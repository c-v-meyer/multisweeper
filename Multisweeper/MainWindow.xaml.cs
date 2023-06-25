using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;

namespace Multisweeper
{
    public partial class MainWindow : Window
    {
        private const byte size = 8;
        private ClientBoard board;
        private Server server;

        public MainWindow(bool createParty)
        {
            board = new ClientBoard(size);
            if (createParty)
            {
                server = new Server(board.size);
                System.Diagnostics.Trace.WriteLine(server.Serve());
            }
            Closing += MainWindow_Closing;
            InitializeComponent();
            DataContext = this;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            server?.Stop();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button) sender;
            Trace.WriteLine(button.Content.ToString());
        }
    }
}