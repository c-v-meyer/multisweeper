using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using System.Collections.Generic;

namespace Multisweeper
{
    public partial class MainWindow : Window
    {
        private const byte size = 8;
        private Server server;
        private Client client;
        private readonly Dictionary<FieldState, string> fileNames =
            new Dictionary<FieldState, string>
            {
                {FieldState.Unrevealed, "/covered_field.png" },
                {FieldState.FlaggedA, "/flag_self.png" },
                {FieldState.FlaggedB, "/flag_opponent.png" },
                {FieldState.RevealedBomb, "/mine.png" },
                {FieldState.Zero, "/revealed_field_0.png" },
                {FieldState.One, "/revealed_field_1.png" },
                {FieldState.Two, "/revealed_field_2.png" },
                {FieldState.Three, "/revealed_field_3.png" },
                {FieldState.Four, "/revealed_field_4.png" },
                {FieldState.Five, "/revealed_field_5.png" },
                {FieldState.Six, "/revealed_field_6.png" },
                {FieldState.Seven, "/revealed_field_7.png" },
                {FieldState.Eight, "/revealed_field_8.png" }
            };

        public MainWindow(bool createParty, string ip)
        {
            InitializeComponent();
            DataContext = this;
            if (createParty)
            {
                server = new Server(size, () => client = new Client("127.0.0.1", false, size, UpdateDisplay, grid.Dispatcher));
                System.Diagnostics.Trace.WriteLine(server.Serve());
            }
            else
            {
                client = new Client(ip, true, size, UpdateDisplay, grid.Dispatcher);
            }
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            server?.Stop();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Image button = (Image) sender;
            Trace.WriteLine(button.Name);

            byte y = (byte)(button.Name[1] - '0');
            byte x = (byte)(button.Name[2] - '0');
            client.Send(ClientMessageType.Reveal,x ,y);
            

        }

        private void UpdateDisplay(ref ClientBoard board)
        {
            
            for(int i = 0; i < board.size; i++)
            {

                for(int j = 0; j< board.size; j++)
                {

                    string s = "";
                    s = "B" + i + j;
                    Image image = grid.FindName(s) as Image;
                    image.Source = new BitmapImage(new System.Uri(fileNames[board.board[i, j]], UriKind.Relative));

                }
            }

        }
    }
}