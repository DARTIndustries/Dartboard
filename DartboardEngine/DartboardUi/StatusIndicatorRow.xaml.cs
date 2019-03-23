using DartboardUi.ConnectionInterfaces;
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
    /// Interaction logic for StatusIndicator.xaml
    /// </summary>
    public partial class StatusIndicatorRow : UserControl, IHandleConnect, IHandleDisconnect
    {
        static SolidColorBrush ConnectedBrush = new SolidColorBrush(Colors.Green);
        static SolidColorBrush DisconnectedBrush = new SolidColorBrush(Colors.Red);

        public StatusIndicatorRow()
        {
            InitializeComponent();
            this.Loaded += (o,e) => ConnectionControl.Bind(this);
            StatusElipse.ToolTip = "Connected";
            StatusElipse.Fill = ConnectedBrush;
        }

        public void HandleDisconnect(Exception ex)
        {
            StatusElipse.ToolTip = $"Disconnected {ex.GetType().Name}";
            StatusElipse.Fill = DisconnectedBrush;
        }

        public void HandleConnect()
        {
            StatusElipse.ToolTip = "Connected";
            StatusElipse.Fill = ConnectedBrush;
        }
    }
}
