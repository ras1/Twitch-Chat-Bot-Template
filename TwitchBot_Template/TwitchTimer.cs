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

        private float RateLimit()
        {
            //Our rate limit is how quickly a bot can send a message back to a user. You can calculate this, but it's easier to just enter a value of 567.
            //Note: This is based off your connection speed.
            //return (float)((20 / 30) * 1000 * 0.85);
            return (567);
        }
    }
}
