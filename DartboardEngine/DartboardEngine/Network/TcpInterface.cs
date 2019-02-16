using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DartboardEngine.Network
{
    public class TcpInterface
    {
        public event Action<Exception> ConnectionException;
        public event Action<Exception> NetworkException;

        public event Action Connected;

        public event Action<byte[]> OnData;


        public TcpClient Client { get; }
        private NetworkStream ClientStream;
        private System.Net.IPEndPoint Endpoint { get; }


        private Thread ClientThread { get; }
        private CancellationTokenSource ThreadCancelToken { get; }

        public TcpInterface(System.Net.IPEndPoint endpoint)
        {
            Endpoint = endpoint;
            ThreadCancelToken = new CancellationTokenSource();
            Client = new TcpClient
            {
                NoDelay = true
            };

            ClientThread = new Thread(ReaderThread);
        }

        public void Start()
        {
            ClientThread.Start();
        }

        public void Shutdown()
        {
            ThreadCancelToken.Cancel();
            if(ClientThread.Join(1000))
            {
                return;
            }

            ClientThread.Abort();
            ClientThread.Join();
            return;
        }


        public void Write(byte[] data, int offset, int length)
        {
            ClientStream.Write(data, offset, length);
        }

        private void ReaderThread()
        {
            bool connected = false;
            while (!connected && !ThreadCancelToken.IsCancellationRequested)
            {
                try
                {
                    Client.Connect(Endpoint);
                    connected = true;
                    Connected?.Invoke();
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception e)
                {
                    ConnectionException?.Invoke(e);
                }
            }
            ClientStream = Client.GetStream();
            byte[] buffer = new byte[2056];
            List<byte> resizeBuffer = new List<byte>();
            while(!ThreadCancelToken.IsCancellationRequested)
            {
                try
                {
                    resizeBuffer.Clear();
                    int readLen;
                    while((readLen = ClientStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        if (readLen == buffer.Length)
                            resizeBuffer.AddRange(buffer);
                        else
                        {
                            resizeBuffer.AddRange(buffer.Take(readLen));
                            break;
                        }
                    }
                    OnData?.Invoke(resizeBuffer.ToArray());
                }
                catch(IOException e)
                {
                    NetworkException?.Invoke(e);
                }
            }
        }
    }
}
