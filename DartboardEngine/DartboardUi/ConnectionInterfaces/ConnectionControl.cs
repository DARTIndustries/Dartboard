using DartboardEngine.Models.Structs;
using DartboardEngine.Network;
using DartboardUi.ConnectionInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DartboardUi
{
    static class ConnectionControl
    {
        public static RobotConnection Connection { get; private set; }

        public static void Setup(IPEndPoint robotEndpoint)
        {
            Connection = new RobotConnection(robotEndpoint);
            Connection.Start();
        }

        public static void Shutdown()
        {
            Connection.Shutdown();
        }

        public static void Send<T>(T item) where T: struct, IRobotCommand
        {
            Connection.Send(item);
        }

        public static void Bind<T>(T control) where T: ContentControl, IHandler
        {
            if(control is IHandleData)
            {
                Connection.OnCommandRecieved += (control as IHandleData).HandleCommand;
                control.Unloaded += (o, e) => Connection.OnCommandRecieved -= (control as IHandleData).HandleCommand;
            }
            if (control is IHandleConnect)
            {
                Connection.OnConnected += (control as IHandleConnect).HandleConnect;
                if(Connection.IsHealthy)
                {
                    (control as IHandleConnect).HandleConnect();
                }
                control.Unloaded += (o, e) => Connection.OnConnected -= (control as IHandleConnect).HandleConnect;
            }
            if (control is IHandleDisconnect)
            {
                Connection.OnDisconnected += (control as IHandleDisconnect).HandleDisconnect;
                if (!Connection.IsHealthy)
                {
                    (control as IHandleDisconnect).HandleDisconnect(Connection.FailureReason);
                }
                control.Unloaded += (o, e) => Connection.OnDisconnected -= (control as IHandleDisconnect).HandleDisconnect;
            }
        }
    }
}
