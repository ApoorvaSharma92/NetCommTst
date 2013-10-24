using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProtoBuf;
using NetCommTst.Common;
using NetworkCommsDotNet;
using System.Net;

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

            Connection connection = TCPConnection.GetConnection(CI);
            string instanceID = "";
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
                MsgSimpleInt msi = new MsgSimpleInt();
                msi.Number = 1776;
                MsgSimpleText mst = remoteObj.AskComplexNumberQuestion(msi);
                Console.WriteLine("Just asked the 8 ball a complex question.  The answer was: " + mst.Text);
            }

        }

        private IMagicEightBall SelectRemoteObject(Connection connection, out string instanceID)
        {
            string instanceNamed ="MyEightBall";

            // Private Client Object Instance
            Console.WriteLine("Creating a Private Client Object Instance ");
            return RemoteProcedureCalls.Client.CreateProxyToPrivateInstance<IMagicEightBall>(connection,instanceNamed,out instanceID);
        }

    }
}
