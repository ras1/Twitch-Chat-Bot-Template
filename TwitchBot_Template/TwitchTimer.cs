using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot_Template
{
    class TwitchTimer
    {
        // So, we need a timer class to limit the amount of messages our bot can send.
        // If you go over the limit you obtain a 2 hour global ban from Twitch.
        // The above is not fun ^^ trust me on this one. ;)
        // Hence, we've created the TwitchTimer class. This class takes all command messages, places them in a queue, and then sends them without going over our message limit.
        // For more information on queues please visit: https://msdn.microsoft.com/en-us/library/7977ey2c(v=vs.110).aspx
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

        public void StartTimer(double messageRateLimit)
        {
            //Starts our timer.
            timer = new System.Timers.Timer(messageRateLimit);
            timer.Elapsed += async (o, s) => await HandleTimer();
            timer.Start();
        }

        private Task HandleTimer()
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
    }
}
