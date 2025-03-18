using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace _0318NetworkProject
{
    public class Message
    {
        public string message;
        public Message(string msg)
        {
            message = msg;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            FileServerProcess();
        }

        static void MessageServerProcess()
        {
            int backlog = 10;

            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 4000);

            listenSocket.Bind(endPoint);
            listenSocket.Listen(backlog);

            Socket clientSocket = listenSocket.Accept();

            byte[] buffer = new byte[1024];
            clientSocket.Receive(buffer);

            string receiveString = Encoding.UTF8.GetString(buffer);
            //Message message = JsonConvert.DeserializeObject<Message>(receiveString);

            Console.WriteLine(receiveString);

            string sendMessage = "반가워요";
            Message msg = new Message(sendMessage);

            byte[] sendBuffer = new byte[1024];
            sendBuffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));

            clientSocket.Send(sendBuffer);

            clientSocket.Close();
            listenSocket.Close();
        }

        static void FileServerProcess()
        {
            int backlog = 10;

            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 4000);

            listenSocket.Bind(endPoint);
            listenSocket.Listen(backlog);

            Socket clientSocket = listenSocket.Accept();

            string path = "./image.webp";
            byte[] imageBuffer = File.ReadAllBytes("./image.webp");
            int index = 0;

            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            bool isCompleted = false;
            while (!isCompleted)
            {
                byte[] buffer = new byte[1024];
                buffer = br.ReadBytes(1024);

                int sendSize = clientSocket.Send(buffer);

                if (sendSize < 1024)
                    isCompleted = true;
                else
                    index += 1024;
            }

            clientSocket.Close();
            listenSocket.Close();
        }
    }
}
