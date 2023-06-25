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
        private Client client;

        public MainWindow(bool createParty, string ip)
        {
            board = new ClientBoard(size);
            if (createParty)
            {
                server = new Server(board.size, () => client = new Client("127.0.0.1", board, UpdateDisplay));
                System.Diagnostics.Trace.WriteLine(server.Serve());
            }
            else
            {
                client = new Client(ip, board, UpdateDisplay);
                // TEST:
                client.Send(ClientMessageType.Reveal, 0, 0);
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
            Image button = (Image) sender;
            Trace.WriteLine(button.Name);
        }

        private void UpdateDisplay()
        {
            // Nimm board und setze Bilder
        }
    }
}