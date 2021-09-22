using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        Socket socket;
        IPEndPoint iPEndPoint;
        List<Socket> clients;
        bool work = true;
        public Server()
        {
            iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clients = new List<Socket>();
        }
        public Server(string ip, int port)
        {
            try
            {
                iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            }
            catch (Exception)
            {
                iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            }

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void Start()
        {
            Console.WriteLine($"Start server [{iPEndPoint.Address}]...");
            try
            {
                socket.Bind(iPEndPoint);
                socket.Listen(10);
                Task senderMsg = new Task(msgSender);
                senderMsg.Start();
                while (work)
                {
                    Connected();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            DisconnectAll();

        }
        private void Connected()
        {
            Socket socketClient = socket.Accept();
            clients.Add(socketClient);
            Console.WriteLine("Registrated new user.");
        }
        public void DisconnectAll()
        {
            clients.ForEach(x => { x.Shutdown(SocketShutdown.Both);x.Close(); }) ;
            socket.Close();
        }
        private void msgSender()
        {
            string msg;
            while(true)
            {
                msg = Console.ReadLine();
                if (msg.ToLower().StartsWith("exit"))
                {
                    work = false;
                    break;
                }
                SendMsgToAll(msg);
                   
            }
        }
        private void SendMsgToAll(string msg)
        {
            clients.ForEach(x => x.Send(Encoding.Unicode.GetBytes(msg)));
        }
    }
}
