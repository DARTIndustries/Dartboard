using DartboardEngine.Models.Structs;
using DartboardEngine.Network;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DartboardUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance {get; private set; }
        public static TcpInterface Interface { get; private set; }

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            Interface = new TcpInterface(new System.Net.IPEndPoint(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), 8400));
            Interface.Start();

            Interface.OnData += OnData;
            Interface.NetworkException += NetException;
            Interface.Connected += Connected;
            Interface.ConnectionException += ConnectException;

            this.Closing += MainWindow_Closing;
        }

        private void ConnectException(Exception obj)
        {
            PushMessage("Connect EX: " + obj.ToString());
        }

        private void Connected()
        {
            PushMessage("Connected!");
        }

        private void NetException(Exception obj)
        {
            PushMessage("Net EX: " + obj.ToString());
        }

        private void OnData(byte[] obj)
        {
            StatusMessage status = StructMarshaller.Decode<StatusMessage>(obj);
            PushMessage("Got Status - CPU: " + status.CpuPercent);
        }

        private void PushMessage(string msg)
        {
            Dispatcher.Invoke(() =>
            {
                Stack.Children.Add(new Label() { Content = msg });
                if(Stack.Children.Count > 10)
                {
                    Stack.Children.RemoveAt(0);
                }
            });
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Interface.Shutdown();
        }
    }
}
