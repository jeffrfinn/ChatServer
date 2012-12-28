using System.IO;
using System.Net;
using System;
using System.Threading;
using Chat = System.Net;
using System.Collections;
using System.Net.Sockets;



namespace Chat_Server
{
    class ChatServer
    {
        TcpListener chatServer;
        public static Hashtable nickName;
        public static Hashtable nickNameByConnect;
        IPAddress host = IPAddress.Parse("localhost");

        public ChatServer()
        {
            nickName = new Hashtable(100);
            nickNameByConnect = new Hashtable(100);

            chatServer = new TcpListener(host, 4296);

            while (true)
            {
                chatServer.Start();

                if (chatServer.Pending())
                {
                    Chat.Sockets.TcpClient chatConnection = chatServer.AcceptTcpClient();

                    Console.WriteLine("You are now connected");

                    DoCommunicate comm = new DoCommunicate(chatConnection);
                }
            }
        }

        public static void SendMsgToAll(string nick, string msg)
        {
            StreamWriter writer;

            ArrayList toRemove = new ArrayList(0);

            Chat.Sockets.TcpClient[] tcpClient = new Chat.Sockets.TcpClient[ChatServer.nickName.Count];

            ChatServer.nickName.Values.CopyTo(tcpClient, 0);

            for (int i = 0; i < tcpClient.Length; i++)
            {
                try
                {
                    if (msg.Trim() == "" || tcpClient[i] == null)
                    {
                        continue;

                        writer.WriteLine(nick + " : " + msg);

                        writer.Flush();

                        writer = null;
                    }
                }
                catch(Exception e44)
                {
                    //e44 = e44;

                    string str = (string)ChatServer.nickNameByConnect[tcpClient[i]];

                    ChatServer.SendSystemMessage("** " + str + " ** Has Left The Room.");

                    ChatServer.nickName.Remove(str);

                    ChatServer.nickNameByConnect.Remove(tcpClient[i]);
                }
            }
        }

        public static void SendSystemMessage(string msg)
        {
            StreamWriter writer;

            ArrayList toRemove = new ArrayList(0);

            Chat.Sockets.TcpClient[] tcpClient = new Chat.Sockets.TcpClient[ChatServer.nickName.Count];

            ChatServer.nickName.Values.CopyTo(tcpClient, 0);

            for (int i = 0; i < tcpClient.Length; i++)
            {
                try
                {
                    if (msg.Trim() == "" || tcpClient[i] == null)
                    {
                        writer = new StreamWriter(tcpClient[i].GetStream());

                        writer.WriteLine(msg);

                        writer.Flush();

                        writer = null;

                    }
                }
                catch(Exception e44)
                {
                    //e44 = e44;

                    ChatServer.nickName.Remove(ChatServer.nickNameByConnect[tcpClient[i]]);

                    ChatServer.nickNameByConnect.Remove(tcpClient[i]);
                }
            }
        }
    }
}
