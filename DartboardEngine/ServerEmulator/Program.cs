using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using DartboardEngine.Models.Structs;
using System.Threading;

namespace ServerEmulator
{
    class Program
    {
        static TcpListener Server;
        static void Main(string[] args)
        {
            Server = new TcpListener(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), 8400)
            {
                
            };
            Server.Start();
            Console.WriteLine("Server Started. Waiting for Client...");
            var client = Server.AcceptTcpClient();

            Console.WriteLine("Client connected. Firing CpuUpdates until dead...");

            var lThread = new Thread(()=>ListenerThread(client));
            lThread.Start();

            Random r = new Random();
            while (client.Connected)
            {
                double perc = 0.6 + r.NextDouble() * .4;
                Console.WriteLine($"Sending CPU: {perc}");
                StatusMessage message = new StatusMessage(DartboardEngine.Models.ECommandType.ROBOT_SYSTEM_STATUS, perc);
                byte[] msgBytes = StructMarshaller.Encode(message);

                client.GetStream().Write(msgBytes, 0, msgBytes.Length);
                Thread.Sleep(1000);
            }
        }

        static void ListenerThread(TcpClient client)
        {
            var stream = client.GetStream();
            while(true)
            {

            }
        }
    }
}
