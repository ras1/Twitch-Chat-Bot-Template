﻿using System;
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
using System.Configuration;
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
        private static string channel = ConfigurationManager.AppSettings.Get("channel");

        NetworkStream serverStream = default(NetworkStream);
        //Declare a new IRCClient
        IrcClient irc = new IrcClient("irc.chat.twitch.tv", 6667);
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
            irc.JoinRoom(); //This is the chat we want our bot to join. Ex: my main channel is funnyguy77 and my bots name is funnybot77. We would join the "funnyguy77" channel.
            chatThread = new Thread(MessageListener); //Starts getting messages from chat.
            chatThread.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Called when we press the X button on our Windows form. 
            //We need to disconnect from the irc and close down the program to avoid leaving anything running in the background.
            irc.LeaveRoom();
            serverStream.Dispose();
            Environment.Exit(0);
        }

        private void MessageListener()
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
                    readData = irc.ReadMessage();
                    ParseMessage();
                }
                catch (Exception e)
                {
                    irc.SendMessage("ERROR: " + e.Message);
                    //Shouldn't be able to throw an exception but if it does we catch it.
                    //Mostly for insurance.
                }
            }
        }

        private void ParseMessage()
        {
            /*Called when we find a new message.
             *This determines what we do with it. In our case, we have a rich text box that where we are displaying the text
             */
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(ParseMessage));
                return;
            }

            string[] separator = new string[] { "#" + channel + " :" }; //This is the same as our ircJoinRoom.
            string[] singlesep = new string[] { ":", "!" };

            // When Twitch sends a message to us it will display something like "PRIVMSG:Blahblahblahblah".
            // This is annoying to read, so if we find this we want to remove it from our text box. It makes things look cleaner.
            if (readData.Contains("PRIVMSG"))
            {
                string messageSender = readData.Split(singlesep, StringSplitOptions.None)[1];
                string message = readData.Split(separator, StringSplitOptions.None)[1];
                chatBox.Text = chatBox.Text + messageSender + " : " + message + Environment.NewLine; //Adds chat text into our rich text box.



                if (message[0] == '!') //Handles commands, in our case when we enter a command we always enter a "!" before it. Hence, we look for that "!".
                {
                    Commands(message);
                }
            }

            if (readData.Contains("PING")) //When Twitch sends us a ping request we have to reply with a pong.
            {
                irc.Pong();
            }
        }

        private void Commands(string message)
        {
            //This method is used to store and run our commands.
            //Note: all of the string separator stuff is used to clean the rich text box we are using. This has nothing to do with Twitch IRC, sending messages, etc.
            string command = message.Split(new[] { ' ', '!' }, StringSplitOptions.None)[1];
            
            string[] singlesep = new string[] { ":", "!" };
            string username = readData.Split(singlesep, StringSplitOptions.None)[1];

            //Store commands here
            //Note: we are storing commands in a switch statement. For more information on switch statements please see: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/switch
            switch (command.ToLower())
            {
                case "ping": //The text you want the user to enter.
                    irc.SendMessage("Pong!"); //This is what the bot will do.
                    break;

                //Note: Default has nothing to do with the above command. This is used to prevent errors that may be caused when users enter "!" but don't specify a command.
                default:
                    break;
            }

        }
        
    }
}
