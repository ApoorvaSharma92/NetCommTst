using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkCommsDotNet;
using System.Net;
using System.Diagnostics;
using System.Threading;

using NetCommTst.Common;

namespace ServerApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            //Trigger the method PrintIncomingMessage when a packet of type 'Message' is received
            //We expect the incoming object to be a string which we state explicitly by using <string>
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Fast", PrintIncomingMessageFast);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("RawFast", PrintIncomingMessageRawFast);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Slow", PrintIncomingMessageSlow);
            NetworkComms.AppendGlobalIncomingPacketHandler<MsgSimpleInt>("SimpleInt", HandleSimpleInt);
            NetworkComms.AppendGlobalIncomingPacketHandler<MsgSimpleInt>("SimpleIntReturnScenario", HandleSimpleIntReturn);

            //Start listening for incoming connections
            //TCPConnection.StartListening(true);

            NetworkComms.PacketConfirmationTimeoutMS = 20000;
            //ConnectionInfo serverConnectionInfo;

            // Define destination address            
            IPEndPoint IPE = IPTools.ParseEndPointFromString("10.1.53.44:19404");
            //serverConnectionInfo = new ConnectionInfo(IPE);

            TCPConnection.StartListening(IPE);
            //TCPConnection.GetConnection(serverConnectionInfo);
            

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
            Console.WriteLine("Fast - " + message );
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
            Console.WriteLine("Slow - " + message );
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
          //  Console.WriteLine("SimpleInt Value was: " + msi.Number.ToString ());
        }


        // Accepts a SimpleInt object and sends back to the calling program a SimpleText object.
        private static void HandleSimpleIntReturn(PacketHeader header, Connection connection, MsgSimpleInt msi)
        {
            MsgSimpleText mst = new MsgSimpleText();
            mst.Text = "The value you sent was: " + msi.Number.ToString();
            Console.WriteLine("SimpleInt Value was: " + msi.Number.ToString() + " sending a SimpleText Object back");
            connection.SendObject("SimpleIntReturnType", mst);

        }
        #endregion
    }
}