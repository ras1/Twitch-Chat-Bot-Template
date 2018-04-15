using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot_Template
{
    class IrcClient
    {
        /* This is our master IRC class, used to make IRC calls.
         * Basically this is where we connect to the Twitch servers and chat.
         * We need to provide a couple things for this to work, see constructor.
         */
        private string channel;

        public TcpClient tcpClient;
        public StreamReader inputStream;
        public StreamWriter outputStream;
        private TwitchTimer twitchTimer;

        //This is a constructor, for more information on constructors visit: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/using-constructors
        //Basically, when we create a new instance of this class we need to obtain a couple things from the user, such as ip, port, userName, and password.
        public IrcClient(string ip, int port, string userName, string password)
        {
            tcpClient = new TcpClient(ip, port);
            inputStream = new StreamReader(tcpClient.GetStream());
            outputStream = new StreamWriter(tcpClient.GetStream());
            twitchTimer = new TwitchTimer(outputStream);
            twitchTimer.StartTimer();

            outputStream.WriteLine("PASS " + password);
            outputStream.WriteLine("NICK " + userName);
            outputStream.WriteLine("USER " + userName + " 8 * :" + userName);
            outputStream.WriteLine("CAP REQ :twitch.tv/membership");
            outputStream.WriteLine("CAP REQ :twitch.tv/commands");
            outputStream.Flush();
        }

        public void JoinRoom(string channel)
        {
            //Called when the bot joins your 'channel' aka room.
            this.channel = channel;
            outputStream.WriteLine("JOIN #" + channel);
            outputStream.Flush();
        }

        public void LeaveRoom()
        {
            //Called when the bot leaves your 'channel' aka room.
            outputStream.Close();
            inputStream.Close();
        }
        public void SendChatMessage(string userName, string message)
        {
            //Called when the bot wants to send a message.
            twitchTimer.SendMessage(":" + userName + "!" + userName + "@" + userName + ".tmi.twitch.tv PRIVMSG #" + channel + " :" + message);
            outputStream.Flush();
        }

        public void Pong()
        {
            //Note: Twitch sends us ping requests every few minutes. We need to respond to these or else we may be kicked from the server.
            twitchTimer.SendMessage("PONG tmi.twitch.tv\r\n");
            outputStream.Flush();
        }

        public string readMessage()
        {
            //Reads messages in Twitch chat.
            string message = "";
            message = inputStream.ReadLine();
            return message;
        }
    }

}
