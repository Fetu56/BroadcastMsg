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
        List<User> users;
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
            socketClient.Send(Encoding.Unicode.GetBytes("Auth on server or login. Log \"[Login] [Pass]\" or \"Reg [Login] [Pass]\" like \"Reg new 123\":"));
            string get = GetString();
            if(get.Split("").Length == 3)
            {
                if (get.StartsWith("Reg"))
                {
                    users.Add(User.Registration(get.Split("")[1], get.Split("")[2], socketClient.AddressFamily.ToString()));
                    clients.Add(socketClient);
                    Console.WriteLine("Registrated new user.");
                }
                else if (get.StartsWith("Log"))
                {
                    if(User.HaveAUser(users, get.Split("")[1], get.Split("")[2], socketClient.AddressFamily.ToString()))
                    {
                        clients.Add(socketClient);
                        Console.WriteLine("Registrated user join.");
                    }
                }
            }
            
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
        private string GetString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            int bytes = 0;
            byte[] data = new byte[256];
            do
            {
                bytes = socket.Receive(data);
                stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (socket.Available > 0);
            return stringBuilder.ToString();
        }
    }
}
