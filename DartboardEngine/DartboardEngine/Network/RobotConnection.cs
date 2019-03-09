using DartboardEngine.Models.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DartboardEngine.Network
{
    public class RobotConnection
    {
        public event Action<ICommand> OnCommandRecieved;
        public event Action<Exception> OnDisconnected;
        public event Action OnReconnected;

        public bool IsHealthy { get; private set; }

        public TcpInterface Interface { get; }
        private readonly ManualResetEventSlim _startupWaitSlim;
        private Exception _startupException;

        private bool _waitingForStartup;

        private readonly PacketParser _parser;

        public RobotConnection(System.Net.IPEndPoint robotEndPoint)
        {
            Interface = new TcpInterface(robotEndPoint);
            _startupWaitSlim = new ManualResetEventSlim();

            _parser = new PacketParser();

            Interface.OnData += _HandleData;
            Interface.NetworkException += _HandleDataException;
            Interface.Connected += _HandleConnection;
            Interface.ConnectionException += _HandleConnectionException;
            _waitingForStartup = false;
            IsHealthy = false;
        }

        public void Start()
        {
            _waitingForStartup = true;
            _startupException = null;
            Interface.Start();
            _startupWaitSlim.Wait();
            if (_startupException != null)
            {
                IsHealthy = false;
                throw _startupException;
            }
            IsHealthy = true;
            _startupWaitSlim.Reset();
            _waitingForStartup = false;
        }

        public void Shutdown()
        {
            Interface.Shutdown();
        }

        public void Send<T>(T command) where T: struct, ICommand
        {
            byte[] arr = StructMarshaller.Encode(command);
            Interface.Write(arr, 0, arr.Length);
        }

        private void _HandleConnectionException(Exception obj)
        {
            IsHealthy = false;
            _parser.ClearBuffer();
            _startupException = obj;
            _startupWaitSlim.Set();
        }

        private void _HandleConnection()
        {
            IsHealthy = true;
            _parser.ClearBuffer();
            _startupException = null;
            _startupWaitSlim.Set();
            OnReconnected.Invoke();
            if(!_waitingForStartup)
            {
                OnReconnected?.Invoke();
            }
        }

        private void _HandleDataException(Exception obj)
        {
            IsHealthy = false;
            _parser.ClearBuffer();
            OnDisconnected.Invoke(obj);
        }

        private void _HandleData(byte[] obj)
        {
            IsHealthy = true;
            foreach (var result in _parser.Parse(obj))
            {
                OnCommandRecieved?.Invoke(result);
            }
        }
    }
}
