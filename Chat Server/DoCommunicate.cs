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
        // global variables
        TcpClient client;
        StreamReader reader;
        StreamWriter writer;

        String nickName;

        public DoCommunicate(TcpClient tcpClient)
        {
            // create tcpClient
            client = tcpClient;
            // create a thread to handle the chat functionality
            Thread chatThread = new Thread(new ThreadStart(StartChat));
            // start the thread
            chatThread.Start();
        }

        private string GetNickName()
        {
            // write line to the user
            writer.WriteLine("What is your name ?");
            // flush any un-written bytes
            writer.Flush();
            // return the user supplied input
            return reader.ReadLine();
        }

        private void StartChat()
        {
            // create our StreamReader object to read the current stream
            reader = new StreamReader(client.GetStream());
            // create our StreamWriter objec to write to the current stream
            writer = new StreamWriter(client.GetStream());
            // write message to the user
            writer.WriteLine("Welcome to chat");
            // set nick name using getNickName method
            nickName = GetNickName();

            // while the nick name already exists
            while (Chat_Server.ChatServer.nickName.Contains(nickName))
            {
                // inform user that that nickname is taken
                writer.WriteLine("ERROR Username already exists please try another");
                // ask the user to supply a new nickname
                nickName = GetNickName();
            }
            
            // add the key nickname and value of tcp client to the nickname hashtable in the ChatServer.cs
            Chat_Server.ChatServer.nickName.Add(nickName, client);
            // add the tcp client as the key and nickname as the value to the nickNameByConnect in ChatServer.cs
            Chat_Server.ChatServer.nickNameByConnect.Add(client, nickName);
            // Send Message to all users informing that a new user has joined the conversation
            Chat_Server.ChatServer.SendSystemMessage("** " + nickName + " ** Has joined the room");
            // send message to user
            writer.WriteLine("Now Talking.....\r\n-------------------------------");
            // flush un-written bytes
            writer.Flush();
            // create a new thread for this user
            Thread chatThread = new Thread(new ThreadStart(RunChat));
            // start the thread
            chatThread.Start();
        }

        private void RunChat()
        {
            //use a try...catch to catch any exceptions
            try
            {
                //set out line variable to an empty string
                string line = "";

                while (true)
                {
                    //read the curent line
                    line = reader.ReadLine();
                    //send our message
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
