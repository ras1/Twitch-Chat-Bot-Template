using System;
using System.Configuration;
using System.IO;
using System.Net.Sockets;

namespace TwitchBot_Template
{
    class IrcClient
    {
        //This is our master IRC class, used to make IRC calls.
        //Basically this is where we connect to the Twitch servers and chat.
        //We need to provide a couple things for this to work, see constructor.
        private static string username = ConfigurationManager.AppSettings.Get("username");
        private static string password = ConfigurationManager.AppSettings.Get("password");
        private static string channel = ConfigurationManager.AppSettings.Get("channel");
        private static double messageRateLimit = Convert.ToDouble(ConfigurationManager.AppSettings.Get("messageRateLimit"));

        public TcpClient tcpClient;
        private StreamReader inputStream;
        private StreamWriter outputStream;
        private TwitchTimer twitchTimer;

        //This is a constructor, for more information on constructors visit: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/using-constructors
        //Basically, when we create a new instance of this class we need to obtain a couple things from the user, such as ip, port, userName, and password.
        public IrcClient(string ip, int port)
        {
            tcpClient = new TcpClient(ip, port);
            inputStream = new StreamReader(tcpClient.GetStream());
            outputStream = new StreamWriter(tcpClient.GetStream());
            twitchTimer = new TwitchTimer(outputStream);
            twitchTimer.StartTimer(messageRateLimit);

            outputStream.WriteLine("PASS " + password);
            outputStream.WriteLine("NICK " + username);
            outputStream.WriteLine("USER " + username + " 8 * :" + username);
            outputStream.WriteLine("CAP REQ :twitch.tv/membership");
            outputStream.WriteLine("CAP REQ :twitch.tv/commands");
            outputStream.Flush();
        }

        public void SendMessage(string message)
        {
            //Called when the bot wants to send a message.
            twitchTimer.SendMessage(":" + username + "!" + username + "@" + username + ".tmi.twitch.tv PRIVMSG #" + channel + " :" + message);
            outputStream.Flush();
        }

        public string ReadMessage()
        {
            //Reads messages in Twitch chat.
            string message = inputStream.ReadLine();
            return message;
        }


        public void JoinRoom()
        {
            //Called when the bot joins your 'channel' aka room.
            outputStream.WriteLine("JOIN #" + channel);
            outputStream.Flush();
        }

        public void LeaveRoom()
        {
            //Called when the bot leaves your 'channel' aka room.
            outputStream.Close();
            inputStream.Close();
        }

        public void Pong()
        {
            //Note: Twitch sends us ping requests every few minutes. We need to respond to these or else we may be kicked from the server.
            twitchTimer.SendMessage("PONG tmi.twitch.tv\r\n");
            outputStream.Flush();
        }
    }

}
