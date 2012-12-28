using System.IO;
using System.Net;
using System;
using System.Threading;
using Chat = System.Net;
using System.Collections;
using Chat_Server;
using System.Net.Sockets;



namespace Chat_Server
{
    class DoCommunicate
    {
        TcpClient client;
        StreamReader reader;
        StreamWriter writer;

        String nickName;

        public DoCommunicate(TcpClient tcpClient)
        {
            client = tcpClient;

            Thread chatThread = new Thread(new ThreadStart(StartChat));

            chatThread.Start();
        }

        private string GetNickName()
        {
            writer.WriteLine("What is your name ?");
            writer.Flush();

            return reader.ReadLine();
        }

        private void StartChat()
        {
            reader = new StreamReader(client.GetStream());

            writer = new StreamWriter(client.GetStream());

            writer.WriteLine("Welcome to chat");

            nickName = GetNickName();

            while (Chat_Server.ChatServer.nickName.Contains(nickName))
            {
                writer.WriteLine("ERROR Username already exists please try another");
                nickName = GetNickName();
            }

            Chat_Server.ChatServer.nickName.Add(nickName, client);

            Chat_Server.ChatServer.nickNameByConnect.Add(client, nickName);

            Chat_Server.ChatServer.SendSystemMessage("** " + nickName + " ** Has joined the room");

            writer.WriteLine("Now Talking.....\r\n-------------------------------");

            writer.Flush();

            Thread chatThread = new Thread(new ThreadStart(RunChat));

            chatThread.Start();
        }

        private void RunChat()
        {
            try
            {
                string line = "";

                while (true)
                {
                    line = reader.ReadLine();

                    Chat_Server.ChatServer.SendMsgToAll(nickName, line);
                }
            }
            catch (Exception e44)
            {
                Console.WriteLine(e44);
            }
        }
    }
}
