using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace _0318NetworkClient
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
            //MessageClientProcess();
            FileClientProcess();
        }

        static void MessageClientProcess()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 4000);

            serverSocket.Connect(endPoint);

            string message = "안녕하세요";
            Message msg = new Message(message);

            byte[] buffer = new byte[1024];

            buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));

            serverSocket.Send(buffer);

            byte[] receiveBuffer = new byte[1024];
            serverSocket.Receive(receiveBuffer);

            string receiveString = Encoding.UTF8.GetString(receiveBuffer);
            //Message receiveMessage = JsonConvert.DeserializeObject<Message>(receiveString);

            Console.WriteLine(receiveString);

            serverSocket.Close();
        }

        static void FileClientProcess()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 4000);

            serverSocket.Connect(endPoint);

            string path = "./image.webp";
            FileStream fs = new FileStream(path, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            //byte[] imageBytes = new byte[100000];

            bool isCompleted = false;
            while(!isCompleted)
            {
                byte[] receiveBuffer = new byte[1024];
                int receiveSize = serverSocket.Receive(receiveBuffer);

                if (receiveSize != 1024)
                {
                    bw.Write(receiveBuffer);
                    isCompleted = true;
                }
                else
                {
                    bw.Write(receiveBuffer);
                }
            }

            bw.Close();
            fs.Close();

            serverSocket.Close();
        }
    }
}
