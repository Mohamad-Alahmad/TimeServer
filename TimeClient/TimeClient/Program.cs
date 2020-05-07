using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TimeClient
{
    class Program
    {
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
            Console.Title = "Client";
            LoopConnect();
            SendLoop();
            Console.ReadKey();
        }

        private static void SendLoop()
        {
            while (true)
            {
                Console.WriteLine("Enter a request : ");
                string request = Console.ReadLine();
                byte[] buffer = Encoding.ASCII.GetBytes(request);
                _clientSocket.Send(buffer);

                byte[] recievedBuffer = new byte[1024];
                int req = _clientSocket.Receive(recievedBuffer);

                byte[] data = new byte[req];
                Array.Copy(recievedBuffer, data, req);
                Console.WriteLine("Recieved : " + Encoding.ASCII.GetString(data));
            }
        }

        private static void LoopConnect()
        {
            int attempts = 0;

            while(!_clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    _clientSocket.Connect(IPAddress.Loopback, 100);
                }
                catch(SocketException)
                {
                    Console.Clear();
                    Console.WriteLine("Connection attempts : " + attempts);
                }
            }

            Console.Clear();
            Console.WriteLine("Connected");
        }
    }  
    
}
