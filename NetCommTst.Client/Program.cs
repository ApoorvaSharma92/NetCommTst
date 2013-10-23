using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DPSBase;
using NetworkCommsDotNet;
using System.Net;
using System.Diagnostics;

namespace ClientApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            NetworkComms.PacketConfirmationTimeoutMS = 20000;
            ConnectionInfo serverConnectionInfo;

            // Define destination address            
            IPEndPoint IPE = IPTools.ParseEndPointFromString("10.1.53.44:19404");
            serverConnectionInfo = new ConnectionInfo(IPE);

            TCPConnection server = TCPConnection.GetConnection(serverConnectionInfo);

            // Setup Send receive options
            SendReceiveOptions nullCompressionSRO = new SendReceiveOptions(DPSManager.GetDataSerializer<ProtobufSerializer>(),
                        new List<DataProcessor>(),
                        new Dictionary<string, string>());            


            Stopwatch sw = new Stopwatch();
            sw.Start();

            StringBuilder sb = new StringBuilder("Start");

            Console.WriteLine("Building string");
            for (int i = 0; i < 500; i++)
                sb.Append("abcdefghij");
            string ms = sb.ToString();
            string ms2 = "Payload: ";

            Console.WriteLine("Testing Network sends");
            int jj = 0;
            int SendQty = 30;
            for (jj=0;jj<SendQty ;jj++)
            {
                //Send the message in a single line
                ms2 = ms2 + jj.ToString();
                server.SendObject("Fast", ms2,nullCompressionSRO );
                server.SendObject("Slow",ms2,nullCompressionSRO);
            }
            sw.Stop();
           
            Console.WriteLine("Network test done");
            Console.WriteLine("Time to send " + jj.ToString() + " messages of " + ms.Length + " was: " + sw.Elapsed.ToString() + " seconds");
            Console.ReadLine();
            //We have used comms so we make sure to call shutdown
            NetworkComms.Shutdown();
        }
    }
}