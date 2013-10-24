using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DPSBase;
using NetworkCommsDotNet;
using System.Net;
using System.Diagnostics;

using NetCommTst.Common;


namespace ClientApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            NetworkComms.PacketConfirmationTimeoutMS = 20000;
            ConnectionInfo serverConnectionInfo;

            // Define destination address            
            IPEndPoint IPE = IPTools.ParseEndPointFromString("127.0.0.1:19525");
            serverConnectionInfo = new ConnectionInfo(IPE);

            TCPConnection server = TCPConnection.GetConnection(serverConnectionInfo);

            int SendQty = 10000;
            SendMsgsWithoutReplies(server,SendQty );
            SendMsgsWaitingForReplies(server,SendQty );
            //SendCustomObjWithTCPReplies(server,SendQty );
            SendCustomObjWithReturnObject(server,SendQty  );

            Console.WriteLine("Press return to exit client.");
            Console.ReadLine();

            //We have used comms so we make sure to call shutdown
            NetworkComms.Shutdown();
        }

        private static void SendMsgsWithoutReplies(TCPConnection server, int Qty)
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
            int SendQty = Qty ;
            try
            {

                for (jj = 0; jj < SendQty; jj++)
                {
                    //Send the message in a single line
                    ms2 = ms2 + jj.ToString();
                    server.SendObject("Fast", ms2, nullCompressionSRO);
                    //server.SendObject("Slow", ms2, nullCompressionSRO);

                }
                double rate;
                double qty = SendQty;
                rate = qty / sw.ElapsedMilliseconds * 1000;
                Console.WriteLine("Network test done");
                Console.WriteLine("Time to send " + jj.ToString() + " Fast messages without replies was: " + sw.Elapsed.ToString() + " seconds.  Rate = " + rate.ToString() + " msgs/sec");
            
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


        }


        private static void SendMsgsWaitingForReplies(TCPConnection server, int qty)
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
            int SendQty = qty ;
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

            double rate;
            double qty2 = SendQty;
            rate = qty2 / sw.ElapsedMilliseconds * 1000;
            Console.WriteLine("Network test done");
            Console.WriteLine("Time to send " + jj.ToString() + " RawFast Objects and receive a TCPIP Reply was: " + sw.Elapsed.ToString() + " seconds.  Rate = " + rate.ToString() + " msgs/sec");
            
        }

        private static void SendCustomObjWithTCPReplies(TCPConnection server, int Qty)
        {
            Console.WriteLine("Sending custom objects and waiting for TCP replies.");

            // Setup Send receive options
            SendReceiveOptions nullCompressionSRO = new SendReceiveOptions(DPSManager.GetDataSerializer<ProtobufSerializer>(),
                        new List<DataProcessor>(),
                        new Dictionary<string, string>());

            // This is the big change in this routine.
            // Results in nearly doubling the runtime...makes sense for each packet sent needs to get one back....
            nullCompressionSRO.ReceiveConfirmationRequired = true;

            // Build Objects
            MsgSimpleInt msi = new MsgSimpleInt();
            msi.Number = 1776;


            Console.WriteLine("Testing Network sends");

            //================================================================
            // Start Actual Sending
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int jj = 0;
            int SendQty = Qty;
            try
            {
                for (jj = 0; jj < SendQty; jj++)
                {
                    server.SendObject("SimpleInt", msi, nullCompressionSRO);
                    //server.SendObject("RawFast", ms2, nullCompressionSRO);
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
            Console.WriteLine("Time to send " + jj.ToString() + " SimpleInt Objects " + " was: " + sw.Elapsed.ToString() + " seconds");
            
        }

        private static void SendCustomObjWithReturnObject(TCPConnection server, int Qty)
        {
            Console.WriteLine("Sending custom objects and waiting for a return objecct!");

            // Setup Send receive options
            SendReceiveOptions ProtoOnly = new SendReceiveOptions (DPSManager.GetDataSerializer<ProtobufSerializer>(),null,null );

            //SendReceiveOptions nullCompressionSRO = new SendReceiveOptions(DPSManager.GetDataSerializer<ProtobufSerializer>(),
              //          new List<DataProcessor>(),
                //        new Dictionary<string, string>());

            // This is the big change in this routine.
            // Results in nearly doubling the runtime...makes sense for each packet sent needs to get one back....
            //nullCompressionSRO.ReceiveConfirmationRequired = true;
            ProtoOnly.ReceiveConfirmationRequired = true;
            // Build Objects
            MsgSimpleInt msi = new MsgSimpleInt();
            msi.Number = 1776;

            MsgSimpleText mst;


            Console.WriteLine("Testing Network sends");

            //================================================================
            // Start Actual Sending
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int jj = 0;
            int SendQty = 100;
            int SubSetQty = Qty / 100;
            try
            {
                for (jj = 0; jj < SendQty; jj++)
                {
                    for (int ii = 0; ii < SubSetQty ; ii++)
                    {
                        // The following command does the following:
                        //  -- Send a MsgSimpleInt object to server (4th parameter)
                        //  -- Sends the server the message SimpleIntReturnScenario - This sends the message to the correct handler on server.
                        //  -- Tells server to return it back in the SimpleIntReturnType message.
                        //  -- timeout after 2500 seconds.
                        //  -- Sends transport options - including serialization protocol.
                        mst = server.SendReceiveObject<MsgSimpleText>("SimpleIntReturnScenario", "SimpleIntReturnType", 2500, msi,ProtoOnly ,ProtoOnly ); //, nullCompressionSRO, nullCompressionSRO);
                        //Console.WriteLine("Received back the following: " + mst.Text);
                    }
                    Console.WriteLine("Just processed 100 * " + jj.ToString() + " records");
                }
            }
            catch (CommunicationException ex)
            {
                int r;
                r = 1;
            }
            catch (ConfirmationTimeoutException ex)
            { int r; r=0; }
            catch (CommsException ex)
            { Console.WriteLine("Bad stuff just happened - " + ex.ToString()); }
            finally { Console.WriteLine("Finally exited"); }
            sw.Stop();

            double rate;
            double qty = SendQty*SubSetQty ;
            rate = qty / sw.ElapsedMilliseconds * 1000;
            Console.WriteLine("Network test done");
            Console.WriteLine("Time to send " + jj.ToString() + " SimpleInt Objects and receive SimpleText objects back was: " + sw.Elapsed.ToString() + " seconds.  Rate = " + rate.ToString () + " msgs/sec");
            
        }
    }
}