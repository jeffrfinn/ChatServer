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
        // listen for connections form tcp clients
        TcpListener chatServer;

        public static Hashtable nickName;
        public static Hashtable nickNameByConnect;


        IPAddress host = IPAddress.Parse("127.0.0.1");
        
        public static void Main()
        {
            ChatServer c = new ChatServer();
        }
        
        public ChatServer()
        {
            // create nickName & nickNameByConnect variables
            nickName = new Hashtable(100);
            nickNameByConnect = new Hashtable(100);

            // initialise chatServer with at the local IP address host on port 4296
            chatServer = new TcpListener(host, 4296);
            // check to see that the server is running

            // visual indication server is running
            Console.WriteLine("Server Running");
            
            //keep running 
            while (true)
            {
                // start the server
                chatServer.Start();

                // if there are connections pending
                if (chatServer.Pending())
                {
                    // accept the connection
                    Chat.Sockets.TcpClient chatConnection = chatServer.AcceptTcpClient();
                    // display message to client
                    Console.WriteLine("You are now connected");
                    // create a new DoCommunicate object
                    DoCommunicate comm = new DoCommunicate(chatConnection);
                }
            }
        }

        // sends a message to all users connected to the server
        public static void SendMsgToAll(string nick, string msg)
        {
            // create a new stream writer
            StreamWriter writer;

            // list of names to remove
            ArrayList toRemove = new ArrayList(0);
            
            // create a new tcpClient array to the size of users held in the hashtable nickName
            Chat.Sockets.TcpClient[] tcpClient = new Chat.Sockets.TcpClient[ChatServer.nickName.Count];

            // copy values from the hashtable nickName to the tcpClient array starting at index 0
            ChatServer.nickName.Values.CopyTo(tcpClient, 0);

            // for every tcp client
            for (int i = 0; i < tcpClient.Length; i++)
            {
                try
                {
                    // if messege is empty or there is no client
                    if (msg.Trim() == "" || tcpClient[i] == null)
                    {
                        /*Use the GetStream method to get the current memory
                        stream for this index of our TCPClient array */
                        writer = new StreamWriter(tcpClient[i].GetStream());
                        // write line nickname : and the message
                        writer.WriteLine(nick + " : " + msg);
                        // flush the stream of any un-written bytes
                        writer.Flush();
                        // reset the stream to null
                        writer = null;

                        //continue;
                    }
                }
                // catch that a user has left the conversation
                catch(Exception e44)
                {
                    e44 = e44;
                    // get nickname of user who has left the conversation
                    string str = (string)ChatServer.nickNameByConnect[tcpClient[i]];
                    // send the message to the other users
                    ChatServer.SendSystemMessage("** " + str + " ** Has Left The Room.");
                    // remove the nickname from the array
                    ChatServer.nickName.Remove(str);
                    // remove the tcp connection 
                    ChatServer.nickNameByConnect.Remove(tcpClient[i]);
                }
            }
        }

        public static void SendSystemMessage(string msg)
        {
            // create a new stream writer
            StreamWriter writer;

            // list of names to remove
            ArrayList toRemove = new ArrayList(0);

            // create a new tcpClient array to the size of users held in the hashtable nickName
            Chat.Sockets.TcpClient[] tcpClient = new Chat.Sockets.TcpClient[ChatServer.nickName.Count];

            // copy values from the hashtable nickName to the tcpClient array starting at index 0
            ChatServer.nickName.Values.CopyTo(tcpClient, 0);

            // for every tcp client
            for (int i = 0; i < tcpClient.Length; i++)
            {
                try
                {
                    // if messege is empty or there is no client
                    if (msg.Trim() == "" || tcpClient[i] == null)
                    {
                        /*Use the GetStream method to get the current memory
                        stream for this index of our TCPClient array */
                        writer = new StreamWriter(tcpClient[i].GetStream());

                        // write message to stream
                        writer.WriteLine(msg);
                        // flush the stream of any un-written bytes
                        writer.Flush();
                        // reset the stream to null
                        writer = null;

                    }
                }
                // catch when a user has left the room
                catch(Exception e44)
                {
                    e44 = e44;
                    // remove the nickname from the array
                    ChatServer.nickName.Remove(ChatServer.nickNameByConnect[tcpClient[i]]);
                    // remove the tcp connection 
                    ChatServer.nickNameByConnect.Remove(tcpClient[i]);
                }
            }
        }
    }
}
