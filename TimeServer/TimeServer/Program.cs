using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TimeServer
{
    class Program
    {
        private static byte[] _buffer = new byte[1024];
        private static List<Socket> _clientSockets = new List<Socket>();
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
            Console.Title = "Server";
            setUpServer();
            Console.ReadKey();
        }

        public static void setUpServer() 
        {
            Console.WriteLine("Setting up server ...");
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 100));
            _serverSocket.Listen(5);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);
            
        }

        private static void AcceptCallBack(IAsyncResult ar) 
        {
            Socket Socket = _serverSocket.EndAccept(ar);
            _clientSockets.Add(Socket);
            Console.WriteLine("Client Connected");
            Socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), Socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);
        }

        private static void RecieveCallback(IAsyncResult ar)
        {
            Socket Socket = (Socket)ar.AsyncState;
            int recieved = Socket.EndReceive(ar);
            byte[] Databuf = new byte[recieved];
            Array.Copy(_buffer, Databuf, recieved);
            string text = Encoding.ASCII.GetString(Databuf);
            Console.WriteLine("Text received : " + text);

            string response = string.Empty;

            if(text.ToLower() != "get time") 
            {
                response = "invalid request !";
            }else
            {
                response = DateTime.Now.ToLongTimeString();
            }

            byte[] data = Encoding.ASCII.GetBytes(response);
            Socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallBack), Socket);
            Socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), Socket);
        }

      

        private static void SendCallBack(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndSend(ar);
        }
    }
}
