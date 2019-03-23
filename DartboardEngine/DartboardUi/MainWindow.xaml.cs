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
        public MainWindow()
        {
            ConnectionControl.Setup(new System.Net.IPEndPoint(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), 8400));
            InitializeComponent();
            Closed += (o, e) => ConnectionControl.Shutdown();
        }
    }
}
