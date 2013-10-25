using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkCommsDotNet;
using System.Net;
using System.Diagnostics;
using System.Threading;

using DPSBase;
using System.Configuration;
using NetCommTst.Common;

namespace ServerApplication
{
    class Program
    {
        static SendReceiveOptions ProtoOnly = new SendReceiveOptions(DPSManager.GetDataSerializer<ProtobufSerializer>(), null, null);

        static void Main(string[] args)
        {
            //Trigger the method PrintIncomingMessage when a packet of type 'Message' is received
            //We expect the incoming object to be a string which we state explicitly by using <string>
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Fast", PrintIncomingMessageFast);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("RawFast", PrintIncomingMessageRawFast);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Slow", PrintIncomingMessageSlow);
            NetworkComms.AppendGlobalIncomingPacketHandler<MsgSimpleInt>("SimpleInt", HandleSimpleInt);
            NetworkComms.AppendGlobalIncomingPacketHandler<MsgSimpleInt>("SimpleIntReturnScenario", HandleSimpleIntReturn);
            NetworkComms.AppendGlobalIncomingPacketHandler<int>("LoadTstInt", HandleLoadTst_Int);
            NetworkComms.AppendGlobalIncomingPacketHandler<double>("LoadTstDouble", HandleLoadTst_Double);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("LoadTstString", HandleLoadTst_String);
            NetworkComms.AppendGlobalIncomingPacketHandler<MsgReallyComplexA>("LoadTstMRCA", HandleLoadTst_MsgReallyComplex);
            NetworkComms.AppendGlobalIncomingPacketHandler<int>("LoadTstLongDelay", HandleLoadTst_LongDelay);
            NetworkComms.AppendGlobalIncomingPacketHandler<int>("LoadTstVeryLongDelay", HandleLoadTst_VeryLongDelay);    

            // Set engine defaults

            // Setup Send receive options
            SendReceiveOptions ProtoOnly = new SendReceiveOptions(DPSManager.GetDataSerializer<ProtobufSerializer>(), null, null);
            NetworkComms.DefaultSendReceiveOptions = ProtoOnly;

            NetworkComms.PacketConfirmationTimeoutMS = 20000;

            // Define destination address         
            string ip = ConfigurationSettings.AppSettings["IP_Address"];
            string Port = ConfigurationSettings.AppSettings["Port"];
            IPEndPoint IPE = IPTools.ParseEndPointFromString(ip + ":" + Port);

            TCPConnection.StartListening(IPE);
            

            //Print out the IPs and ports we are now listening on
            Console.WriteLine("Server listening for TCP connection on:");
            foreach (System.Net.IPEndPoint localEndPoint in TCPConnection.ExistingLocalListenEndPoints()) Console.WriteLine("{0}:{1}", localEndPoint.Address, localEndPoint.Port);

            //Let the user close the server
            Console.WriteLine("\nPress any key to close server.");
            Console.ReadKey(true);

            //We have used NetworkComms so we should ensure that we correctly call shutdown
            NetworkComms.Shutdown();
        }

        #region MsgHandlers
        /// <summary>
        /// Writes the provided message to the console window
        /// </summary>
        /// <param name="header">The packetheader associated with the incoming message</param>
        /// <param name="connection">The connection used by the incoming message</param>
        /// <param name="message">The message to be printed to the console</param>
        private static void PrintIncomingMessageFast(PacketHeader header, Connection connection, string message)
        {
          //  Console.WriteLine("Fast - " + message );
           // Console.WriteLine("\nA message was recieved from " + connection.ToString() + " which said '" + message + "'.");
        }


        /// <summary>
        /// Receiver for the Slow message test case.  Sleeps for 8 seconds and then wakes up
        /// </summary>
        /// <param name="header"></param>
        /// <param name="connection"></param>
        /// <param name="message"></param>
        private static void PrintIncomingMessageSlow(PacketHeader header, Connection connection, string message)
        {
            Console.WriteLine("Connection from: " + connection.ConnectionInfo.RemoteEndPoint.ToString() + " is going to slow queue..." + message);
            Thread.Sleep(8000);
            Console.WriteLine("Waking up!");
            // Console.WriteLine("\nA message was recieved from " + connection.ToString() + " which said '" + message + "'.");
        }

        /// <summary>
        /// Receiver for the RawFast test routines
        /// </summary>
        /// <param name="header"></param>
        /// <param name="connection"></param>
        /// <param name="message"></param>
        private static void PrintIncomingMessageRawFast(PacketHeader header, Connection connection, string message)
        {
        }

        /// <summary>
        ///Accepts a SimpleInt object.  Does nothing with it, purely a performance test routine 
        /// </summary>
        /// <param name="header"></param>
        /// <param name="connection"></param>
        /// <param name="msi">SimpleInt object</param>
        private static void HandleSimpleInt(PacketHeader header, Connection connection, MsgSimpleInt msi)
        {
            Console.WriteLine("Connection from: " + connection.ConnectionInfo.RemoteEndPoint.ToString() + " was just processed");
          //  Console.WriteLine("SimpleInt Value was: " + msi.Number.ToString ());
        }


        // Accepts a SimpleInt object and sends back to the calling program a SimpleText object.
        private static void HandleSimpleIntReturn(PacketHeader header, Connection connection, MsgSimpleInt msi)
        {
            //Thread.Sleep(4000);   // Test timeout situation.
            MsgSimpleText mst = new MsgSimpleText();
            mst.Text = "The value you sent was: " + msi.Number.ToString();
            Console.WriteLine("SimpleInt Value was: " + msi.Number.ToString() + " sending a SimpleText Object back");
            connection.SendObject("SimpleIntReturnType", mst,ProtoOnly );
        }
        #endregion

#region LoadTestHandlers
        private static void HandleLoadTst_Int(PacketHeader header, Connection connection, int value)
        {
            value = value * 12;
            //Console.WriteLine("Connection from: " + connection.ConnectionInfo.RemoteEndPoint.ToString() + " was just processed: " + value.ToString() );
        }

        private static void HandleLoadTst_MsgReallyComplex(PacketHeader header, Connection connection, MsgReallyComplexA mrca)
        {
            //Console.WriteLine("Connection from: " + connection.ConnectionInfo.RemoteEndPoint.ToString() + " sent us a complex object with year value of: " + mrca.Year.ToString());
        }
        private static void HandleLoadTst_LongDelay(PacketHeader header, Connection connection, int value)
        {
            //Console.WriteLine("Connection from: " + connection.ConnectionInfo.RemoteEndPoint.ToString() + " zzzzzz");
            Thread.Sleep(3000);
            //Console.WriteLine("Awake!!");
        }
        private static void HandleLoadTst_VeryLongDelay(PacketHeader header, Connection connection, int value)
        {
            // Console.WriteLine("Connection from: " + connection.ConnectionInfo.RemoteEndPoint.ToString() + " zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz.");
            Thread.Sleep(15000);
            //   Console.WriteLine("Woke up from long sleep");
        }

        #region LoadTestSendRecvHandlers
        private static void HandleLoadTst_Double(PacketHeader header, Connection connection, double value)
        {
            int r = 0;
            r = (int)value;
                        
            connection.SendObject("RCBool", r, ProtoOnly);
            //Console.WriteLine("Connection from: " + connection.ConnectionInfo.RemoteEndPoint.ToString() + ".  Double Value was: " + value.ToString());
        }
        private static void HandleLoadTst_String(PacketHeader header, Connection connection, string value)
        {          
            //Console.WriteLine("Connection from: " + connection.ConnectionInfo.RemoteEndPoint.ToString() + " sent us a string - " + value);
          //  Thread.Sleep(2000);
            connection.SendObject("RCBool", true, ProtoOnly);
        }
        #endregion


 
#endregion
    }
}