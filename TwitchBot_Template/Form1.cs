using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//Note: We need to add the following using statements.
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
//End using

namespace TwitchBot_Template
{

    /* Welcome to my Twitch Chat Bot template for C#.
     * This should introduce you into making a twitch bot with C#.
     * Please note: This is NOT a tutorial. This should be considered a template.
     * -------------------------------------------------------------------------------
     * Questions? 
     * Visit me at: https://www.twitch.tv/funnyguy77
     * Last Updated: 7-16-2017
     */

    public partial class Form1 : Form
    {
        //Global variables
        //Note: These are kept here because we need to access them throughout the class.
        private static string userName = "twitchBotUsernameHere";
        private static string mainChannelUserName = "mainTwitchChannelUsernameHere"; //This is the username of the channel we wish to connect to.

        //Twitch oauth token goes here, this is used to sign into your bot account.
        //Note: You can generate an oauth token here: https://twitchapps.com/tmi/
        private static string password = "oauth:##########";

        //Declare a new IRCClient
        IrcClient irc = new IrcClient("irc.chat.twitch.tv", 6667, userName, password);
        NetworkStream serverStream = default(NetworkStream);
        Thread chatThread;
        string readData = "";

        public Form1()
        {
            //Initializes Windows form.
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //This method is called when we load our Windows form. 
            irc.joinRoom(mainChannelUserName); //This is the chat we want our bot to join. Ex: my main channel is funnyguy77 and my bots name is funnybot77. We would join the "funnyguy77" channel.
            chatThread = new Thread(getMessage); //Starts getting messages from chat.
            chatThread.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Called when we press the X button on our Windows form. 
            //We need to disconnect from the irc and close down the program to avoid leaving anything running in the background.
            irc.leaveRoom();
            serverStream.Dispose();
            Environment.Exit(0);
        }

        private void getMessage()
        {
            //Constantly gets new messages from chat.
            serverStream = irc.tcpClient.GetStream();
            int bufferSize = 0;
            byte[] inStream = new byte[10025];
            bufferSize = irc.tcpClient.ReceiveBufferSize;
            //While the bot is running cycle threw this.
            while (true)
            {
                try
                {
                    readData = irc.readMessage();
                    msg();
                }
                catch (Exception e)
                {
                    irc.sendChatMessage("ERROR: " + e.Message);
                    //Shouldn't be able to throw an exception but if it does we catch it.
                    //Mostly for insurance.
                }
            }
        }

        private void msg()
        {
            /*Called when we find a new message.
             *This determines what we do with it. In our case, we have a rich text box that where we are displaying the text
             */
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(msg));
            }
            else
            {
                string[] separator = new string[] { "#" + mainChannelUserName + " :" }; //This is the same as our ircJoinRoom.
                string[] singlesep = new string[] { ":", "!" };

                /* When Twitch sends a message to us it will display something like "PRIVMSG:Blahblahblahblah".
                 * This is annoying to read, so if we find this we want to remove it from our text box. It makes things look cleaner.
                 */
                if (readData.Contains("PRIVMSG"))
                {
                    string username = readData.Split(singlesep, StringSplitOptions.None)[1];
                    string message = readData.Split(separator, StringSplitOptions.None)[1];
                    chatBox.Text = chatBox.Text + username + " : " + message + Environment.NewLine; //Adds chat text into our rich text box.

                    if (message[0] == '!') //Handles commands, in our case when we enter a command we always enter a "!" before it. Hence, we look for that "!".
                    {
                        commands(userName, message);
                    }
                }

                if (readData.Contains("PING")) //When Twitch sends us a ping request we have to reply with a pong.
                {
                    irc.pingResponse();
                }
            }
        }

        private void commands(string userName, string message)
        {
            //This method is used to store and run our commands.
            //Note: all of the string separator stuff is used to clean the rich text box we are using. This has nothing to do with Twitch IRC, sending messages, etc.
            string command = message.Split(new[] { ' ', '!' }, StringSplitOptions.None)[1];
            string[] separator = new string[] { "#" + mainChannelUserName + " :" };
            string[] singlesep = new string[] { ":", "!" };
            string username = readData.Split(singlesep, StringSplitOptions.None)[1];

            //Store commands here
            //Note: we are storing commands in a switch statement. For more information on switch statements please see: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/switch
            switch (command.ToLower())
            {
                //Time to add commands!
                //You can add commands here by using the following format.
                case "ping": //The text you want the user to enter.
                    irc.sendChatMessage("Pong!"); //This is what the bot will do.
                    break; //This escapes out of our switch statement.

                //Note: Default has nothing to do with the above command. This is used to prevent errors that may be caused when users enter "!" but don't specify a command.
                default:
                    break;
            }

        }

        class IrcClient
        {
            /* This is our master IRC class, used to make IRC calls.
             * Basically this is where we connect to the Twitch servers and chat.
             * We need to provide a couple things for this to work, see constructor.
             */
            private string userName;
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

            public void joinRoom(string channel)
            {
                //Called when the bot joins your 'channel' aka room.
                this.channel = channel;
                outputStream.WriteLine("JOIN #" + channel);
                outputStream.Flush();
            }

            public void leaveRoom()
            {
                //Called when the bot leaves your 'channel' aka room.
                outputStream.Close();
                inputStream.Close();
            }

            public void sendIrcMessage(string message)
            {
                //Called when the bot wants to send a message.
                twitchTimer.SendMessage(message);
                outputStream.Flush();
            }

            public void sendChatMessage(string message)
            {
                //Called when the bot wants to send a message.
                twitchTimer.SendMessage(":" + userName + "!" + userName + "@" + userName + ".tmi.twitch.tv PRIVMSG #" + channel + " :" + message);
            }

            public void pingResponse()
            {
                //Note: Twitch sends us ping requests every few minutes. We need to respond to these or else we may be kicked from the server.
                sendIrcMessage("PONG tmi.twitch.tv\r\n");
            }

            public string readMessage()
            {
                //Reads messages in Twitch chat.
                string message = "";
                message = inputStream.ReadLine();
                return message;
            }
        }

        class TwitchTimer
        {
            /* When using Twitch, a moderator can only send a maximum of 100 messages every 30 seconds. 
             * A normal user can only send a maximum of 20 messages within 30 seconds.
             * If you go over this limit you obtain a 2 hour global ban from Twitch.
             * The above is not fun ^^ trust me on this one. ;)
             * ------------------------------------------------------------------------
             * So, we need a timer class to limit the amount of messages our bot can send.
             * Hence, we've created the TwitchTimer class. This class takes all command messages, places them in a queue, and then sends them without going over our message limit.
             * For more information on queues please visit: https://msdn.microsoft.com/en-us/library/7977ey2c(v=vs.110).aspx
             */

            public System.Timers.Timer timer;
            public Queue<string> outgoingMessages = new Queue<string>();
            public StreamWriter streamWriter;

            public TwitchTimer(StreamWriter streamwriter)
            {
                //Constructor
                this.streamWriter = streamwriter;
            }

            public void SendMessage(string message)
            {
                //Sends out messages.
                outgoingMessages.Enqueue(message);
            }

            public void StartTimer()
            {
                //Starts our timer.
                timer = new System.Timers.Timer(RateLimit());
                timer.Elapsed += async (o, s) => await Handletimer();
                timer.Start();
            }

            private Task Handletimer()
            {
                if (outgoingMessages.Count > 0)
                {
                    var message = outgoingMessages.Dequeue();
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        streamWriter.WriteLine(message);
                        streamWriter.Flush();
                    }
                }
                return Task.FromResult<object>(null);
            }

            private float RateLimit()
            {
                //Our rate limit is how quickly a bot can send a message back to a user. You can calculate this, but it's easier to just enter a value of 567.
                //Note: This is based off your connection speed.
                //return (float)((20 / 30) * 1000 * 0.85);
                return (567);
            }
        }
    }
}
