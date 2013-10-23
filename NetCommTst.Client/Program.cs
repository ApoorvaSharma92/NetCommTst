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

            //SendMsgsWithoutReplies(server);
            SendMsgsWaitingForReplies(server);

            //We have used comms so we make sure to call shutdown
            NetworkComms.Shutdown();
        }

        private static void SendMsgsWithoutReplies(TCPConnection server)
        {
            Console.WriteLine("Sending messages without waiting for replies - Send as Fast As Possible.");
            // Setup Send receive options
            SendReceiveOptions nullCompressionSRO = new SendReceiveOptions(DPSManager.GetDataSerializer<ProtobufSerializer>(),
                        new List<DataProcessor>(),
                        new Dictionary<string, string>());



            StringBuilder sb = new StringBuilder("Start");

            Console.WriteLine("Building string");
            for (int i = 0; i < 500; i++)
                sb.Append("abcdefghij");
            string ms = sb.ToString();
            string ms2 = "Payload: ";

            Console.WriteLine("Testing Network sends");

            //================================================================
            // Start Actual Sending
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int jj = 0;
            int SendQty = 3000;
            try
            {

                for (jj = 0; jj < SendQty; jj++)
                {
                    //Send the message in a single line
                    ms2 = ms2 + jj.ToString();
                    server.SendObject("Fast", ms2, nullCompressionSRO);
                    server.SendObject("Slow", ms2, nullCompressionSRO);

                }
            }
            catch (CommunicationException ex)
            {
            }
            catch (ConfirmationTimeoutException ex)
            { }
            catch (CommsException ex)
            { Console.WriteLine("Bad stuff just happened - " + ex.ToString()); }
            finally { Console.WriteLine("Finally exited"); }

            sw.Stop();

            Console.WriteLine("Network test done");
            Console.WriteLine("Time to send " + jj.ToString() + " messages of " + ms.Length + " was: " + sw.Elapsed.ToString() + " seconds");
            Console.ReadLine();
        }


        private static void SendMsgsWaitingForReplies(TCPConnection server)
        {
            Console.WriteLine("Sending messages waiting for replies.");

            // Setup Send receive options
            SendReceiveOptions nullCompressionSRO = new SendReceiveOptions(DPSManager.GetDataSerializer<ProtobufSerializer>(),
                        new List<DataProcessor>(),
                        new Dictionary<string, string>());

            // This is the big change in this routine.
            nullCompressionSRO.ReceiveConfirmationRequired = true;

            StringBuilder sb = new StringBuilder("Start");

            Console.WriteLine("Building string");
            for (int i = 0; i < 500; i++)
                sb.Append("abcdefghij");
            string ms = sb.ToString();
            string ms2 = "Payload: ";

            Console.WriteLine("Testing Network sends");

            //================================================================
            // Start Actual Sending
            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            int jj = 0;
            int SendQty = 100000;
            try {
            for (jj = 0; jj < SendQty; jj++)
            {
                server.SendObject("RawFast", ms2, nullCompressionSRO);
            }
            }
            catch (CommunicationException ex)
            {
            }
            catch (ConfirmationTimeoutException ex)
            { }
            catch (CommsException ex)
            { Console.WriteLine("Bad stuff just happened - " + ex.ToString()); }
            finally { Console.WriteLine("Finally exited"); }
            sw.Stop();

            Console.WriteLine("Network test done");
            Console.WriteLine("Time to send " + jj.ToString() + " messages of " + ms.Length + " was: " + sw.Elapsed.ToString() + " seconds");
            Console.ReadLine();
        }
    }
}