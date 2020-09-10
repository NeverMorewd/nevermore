using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace nevermore.common
{
    public class Pinger
    {
        public static async Task TestPing(string aUri)
        {
            System.Net.NetworkInformation.Ping pinger = new System.Net.NetworkInformation.Ping();
            //pinger.PingCompleted += Pinger_PingCompleted;
            PingReply reply = await pinger.SendPingAsync(aUri);

        }

        private static void Pinger_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            
        }
    }
}
