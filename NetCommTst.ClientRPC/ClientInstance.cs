﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProtoBuf;
using NetCommTst.Common;
using NetworkCommsDotNet;
using RemoteProcedureCalls;
using DPSBase;

using System.Net;
using System.Diagnostics;

namespace NetCommTst.ClientRPC
{
    public class ClientInstance
    {
        private ConnectionType cxn;

        public void Start()
        {
            Console.WriteLine("Establishing a TCP Connection...");

            cxn = ConnectionType.TCP;
            IPEndPoint IPE = IPTools.ParseEndPointFromString("127.0.0.1:19504");
            ConnectionInfo CI = new ConnectionInfo(IPE);
            
            NetworkComms.PacketConfirmationTimeoutMS = 20000;
            SendReceiveOptions nullCompressionSRO = new SendReceiveOptions(DPSManager.GetDataSerializer<ProtobufSerializer>(),
                        new List<DataProcessor>(),
                        new Dictionary<string, string>());


            Connection connection = TCPConnection.GetConnection(CI,nullCompressionSRO );
            NetworkComms.DisableConnectionSendTimeouts = true;
            string instanceID = "";


            // Establish RPC connection with Server
            IMagicEightBall remoteObj = SelectRemoteObject(connection, out instanceID);
            Console.WriteLine("RPC instance has been created with instanceID: " + instanceID);

            // Now play with the 8 ball.
            //while (true)
            {
                // Send RPC commands.
                Console.WriteLine ("Resetting result was: " +  remoteObj.Reset());
                Console.WriteLine ("Will I win the Lottery? Answer: " + remoteObj.AskQuestion ("Will I win the lottery?"));
                Console.WriteLine("Result of shaking: " + remoteObj.Shake().ToString());
                Console.WriteLine("Complain at the 8 ball: " + remoteObj.Complain("You cheat!"));

                
                // This demonstrates how to change the RPC timeout for the next call and then reset back.
                IRPCProxy yyyy = (IRPCProxy)remoteObj;
                yyyy.RPCTimeout = 3500;

                
                MsgSimpleInt msi = new MsgSimpleInt();
                msi.Number = 1776;
                MsgSimpleText mst = remoteObj.AskComplexNumberQuestion(msi);
                Console.WriteLine("Just asked the 8 ball a complex question.  The answer was: " + mst.Text);

                // Reset proxy timeout to 10s.
                yyyy.RPCTimeout = 10000;
            }

            // Load Testing - do a bunch of RPC calls.
            Console.WriteLine("Now preparing to loadtest RPC calls...");
            Stopwatch sw = new Stopwatch();
            MsgSimpleText  mst2 = new MsgSimpleText();
            MsgSimpleInt msi2 = new MsgSimpleInt();
            msi2.Number = 1776;

            int SendQty = 30000;
            sw.Start();
            for (int jj = 0; jj < SendQty; jj++)
            {
                mst2 = remoteObj.AskComplexNumberQuestion(msi2);
            }
            sw.Stop();
            double qty = SendQty;
            double rate = (qty / sw.ElapsedMilliseconds) * 1000;
            Console.WriteLine("Just sent " + SendQty.ToString() + " rpc calls to server in " + sw.Elapsed.ToString() + " seconds.  A rate of " + rate.ToString());
            Console.ReadLine();
        }

        private IMagicEightBall SelectRemoteObject(Connection connection, out string instanceID)
        {
            string instanceNamed ="MyEightBall";

            // Private Client Object Instance
            Console.WriteLine("Creating a Private Client Object Instance ");

            IMagicEightBall x;
            IRPCProxy y;

            // Get the Proxy object so we can set some default values for all connections on this proxy.
            y = (IRPCProxy)Client.CreateProxyToPrivateInstance<IMagicEightBall>(connection, instanceNamed, out instanceID);
            x = RemoteProcedureCalls.Client.CreateProxyToPrivateInstance<IMagicEightBall>(connection,instanceNamed,out instanceID);

            // Set RPC timeout to 10s.
            y.RPCTimeout = 10000;

            // Return the RPC interface.
            return ((IMagicEightBall)y);
        }

    }
}
