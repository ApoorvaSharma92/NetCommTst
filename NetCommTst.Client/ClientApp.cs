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
            #region Setup

            // Speed up transactions 40x.  
            SendReceiveOptions ProtoBufSerialize_Only = new SendReceiveOptions(DPSManager.GetDataSerializer<ProtobufSerializer>(),
                        new List<DataProcessor>(),
                        new Dictionary<string, string>());
            // Probably want this set to true always.
            ProtoBufSerialize_Only.ReceiveConfirmationRequired = true;
            NetworkComms.DefaultSendReceiveOptions = ProtoBufSerialize_Only;


            NetworkComms.PacketConfirmationTimeoutMS = 20000;
            ConnectionInfo serverConnectionInfo;

            // Define destination address            
            IPEndPoint IPE = IPTools.ParseEndPointFromString("127.0.0.1:19525");
            serverConnectionInfo = new ConnectionInfo(IPE);

            TCPConnection server = TCPConnection.GetConnection(serverConnectionInfo);

            #endregion
            int SendQty = 10000;

            // Uncomment the test you wish to run.

            //SendMsgsWaitingForReplies(server,SendQty );
            //SendMsgsNoWaitingForReplies(server,SendQty );
            //SendCustomObjWithTCPReplies(server,SendQty );
            //SendCustomObjWithReturnObject(server,SendQty  );

            // Real world load testing.
            LoadTestRandomCalls(server );

            Console.WriteLine("Press return to exit client.");
            Console.ReadLine();

            //We have used comms so we make sure to call shutdown
            NetworkComms.Shutdown();
        }

        private static void SendMsgsWaitingForReplies(TCPConnection server, int Qty)
        {
            Console.WriteLine("Sending messages waiting for replies");
            // Setup Send receive options - We now set it up as app default.  See main routine.
            //SendReceiveOptions nullCompressionSRO = new SendReceiveOptions(DPSManager.GetDataSerializer<ProtobufSerializer>(),
            //            new List<DataProcessor>(),
            //            new Dictionary<string, string>());

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
                    server.SendObject("Fast", ms2);
                    // Below was necessary before we setup no compression and encryption at app default level.  Shows how you can override on a 
                    // per call basis.
                    //server.SendObject("Fast", ms2, nullCompressionSRO);
                    //server.SendObject("Slow", ms2, nullCompressionSRO);

                }
                double rate;
                double qty = SendQty;
                rate = qty / sw.ElapsedMilliseconds * 1000;
                Console.WriteLine("Network test done");
                Console.WriteLine("Time to send " + jj.ToString() + " Fast messages waiting for replies was: " + sw.Elapsed.ToString() + " seconds.  Rate = " + rate.ToString() + " msgs/sec");
            
            }

            catch (CommsException ex)
            { Console.WriteLine("Bad stuff just happened - " + ex.ToString()); }
            finally { Console.WriteLine("Finally exited"); }

            sw.Stop();


        }


        private static void SendMsgsNoWaitingForReplies(TCPConnection server, int qty)
        {
            Console.WriteLine("Sending messages NOT waiting for TCP/IP replies.  - Send as Fast As Possible.");

            // Setup Send receive options
            SendReceiveOptions nullCompressionSRO = new SendReceiveOptions(DPSManager.GetDataSerializer<ProtobufSerializer>(),
                        new List<DataProcessor>(),
                        new Dictionary<string, string>());

            // This is the big change in this routine. 
            nullCompressionSRO.ReceiveConfirmationRequired = false ;

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
                Console.WriteLine(ex.InnerException.ToString());
            }
            catch (ConfirmationTimeoutException ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
            }
            catch (CommsException ex)
            { Console.WriteLine("Bad stuff just happened - " + ex.ToString()); }
            finally { Console.WriteLine("Finally exited"); }
            sw.Stop();

            double rate;
            double qty2 = SendQty;
            rate = qty2 / sw.ElapsedMilliseconds * 1000;
            Console.WriteLine("Network test done");
            Console.WriteLine("Time to send " + jj.ToString() + " RawFast Objects with no TCPIP Reply Wait was: " + sw.Elapsed.ToString() + " seconds.  Rate = " + rate.ToString() + " msgs/sec");
            
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
                Console.WriteLine(ex.InnerException.ToString());
            }
            catch (ConfirmationTimeoutException ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
            }
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

        static void LoadTestRandomCalls(TCPConnection server)
        {
            #region StartupCode
            Console.WriteLine("Load Test with Random calls.");

            // Setup Send receive options
            //SendReceiveOptions ProtoOnly = new SendReceiveOptions(DPSManager.GetDataSerializer<ProtobufSerializer>(), null, null);
            //NetworkComms.DefaultSendReceiveOptions = ProtoOnly;

            // This is the big change in this routine.
            // Results in nearly doubling the runtime...makes sense for each packet sent needs to get one back....
            //nullCompressionSRO.ReceiveConfirmationRequired = true;
            //ProtoOnly.ReceiveConfirmationRequired = true;

            
            // Build Objects we will be using.
            MsgSimpleInt msi = new MsgSimpleInt();
            MsgSimpleText mst = new MsgSimpleText();
            msi.Number = 0;

            Console.WriteLine("Starting Test Now.");

            //================================================================
            // Start Actual Sending
            Stopwatch sw = new Stopwatch();
            sw.Start();
            #endregion


            // Setup some variables
            double SendDbl = 576434.34;
            string SendStr = "Some text is being sent";
            MsgReallyComplexA mrca = new MsgReallyComplexA();
            mrca.Year = 2456;
            mrca.msi = new MsgSimpleInt();
            mrca.msi.Number = 660099;
            mrca.mca = new MSgComplexA();
            mrca.mca.Text = "Hi";
            mrca.mca.Number = -56;

            int jj = 0;
            int kk = 0;
            int ll = 0;
            long ld = 0;
            long vld = 0;

            long c1 = 0;
            long c2 = 0;
            long c3 = 0;
            long c4 = 0;
            long c5 = 0;
            long c6 = 0;

            Random rand = new Random();
            int val;

            int ReportingInterval = 10000;
            int SendQty = ReportingInterval * 1000000; 
            try
            {
                for (jj = 0; jj < SendQty; jj++)
                {
                    Stopwatch sw2 = new Stopwatch();
                    sw2.Start();
                    for (kk = 0; kk < ReportingInterval; kk++)
                    {
                        // Change the max value to 7 to run the sleeping threads server functions.
                        // Change to 5 to run normal routines.
                        val = rand.Next(1, 5);
                        switch (val)
                        {
                            case 1:
                                c1++;
                                server.SendObject("LoadTstInt", val);
                                break;
                            case 2:
                                // Send a double and get an integer back!
                                c2++;
                                int rcint = server.SendReceiveObject<int>("LoadTstDouble", "RCBool", 4000, SendDbl);
                                //Console.WriteLine("Sent a double and got an Int back! = " + rcint.ToString());
                                //server.SendObject("LoadTstDouble", SendDbl);
                                break;
                            case 3:
                                c3++;
                                // Send a string and accept a bool back.
                                bool rc = server.SendReceiveObject<bool>("LoadTstString", "RCBool", 5000, SendStr);
                              //  Console.WriteLine("Received " + rc.ToString());
                                //server.SendObject("LoadTstString", SendStr);
                                break;
                            case 4:
                                c4++;
                                server.SendObject("LoadTstMRCA", mrca);
                                break;
                            case 5:
                                c5++;
                                server.SendObject("LoadTstLongDelay", val);
                                ld++;
                                break;
                            case 6:
                                c6++;
                                server.SendObject("LoadTstVeryLongDelay", val);
                                vld++;
                                break;
                        }
                        //mst = server.SendReceiveObject<MsgSimpleText>("SimpleIntReturnScenario", "SimpleIntReturnType", 2500, msi, ProtoOnly, ProtoOnly); //, nullCompressionSRO, nullCompressionSRO);
                        //Console.WriteLine("Received back the following: " + mst.Text);
                    } // for kk
                    sw2.Stop();

                    Console.WriteLine("Just processed " + kk.ToString() + " records in " + sw2.Elapsed.ToString() + " seconds");
                    Console.WriteLine("c1: " + c1.ToString());
                    Console.WriteLine("c2: " + c2.ToString());
                    Console.WriteLine("c3: " + c3.ToString());
                    Console.WriteLine("c4: " + c4.ToString());
                    Console.WriteLine("c5: " + c5.ToString());
                    Console.WriteLine("c6: " + c6.ToString());
                    c6 = c5 = c4 = c3 = c2 = c1 = 0;
                } // for jj
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
            }
            catch (ConfirmationTimeoutException ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
            }
            catch (CommsException ex)
            { Console.WriteLine("Bad stuff just happened - " + ex.ToString()); }
            finally { Console.WriteLine("Finally exited"); }

            sw.Stop();

            Console.WriteLine("Sent: " + jj.ToString() + " * " + kk.ToString() + " objects to server.");
            Console.WriteLine("This took: " + sw.Elapsed.ToString() + " time to execute");
            Console.WriteLine("We incurred " + ld.ToString() + " long delays and " + vld.ToString() + " very long delays during this time.");
            Console.WriteLine("Network test done");
           
        }
    }
}