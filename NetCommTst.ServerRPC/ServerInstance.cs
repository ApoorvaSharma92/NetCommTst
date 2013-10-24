using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProtoBuf;
using NetCommTst.Common;
using NetworkCommsDotNet;
using System.Net;

namespace NetCommTst.ServerRPC
{
    public class ServerInstance 
    {
        private ConnectionType cxn;


        public ServerInstance()
        { }

        public void Start()
        {
            SetConnectionType();
            SetupRPC();

            if (cxn == ConnectionType.TCP)
                foreach (System.Net.IPEndPoint localEndPoint in TCPConnection.ExistingLocalListenEndPoints()) Console.WriteLine("{0}:{1}", localEndPoint.Address, localEndPoint.Port);
           
           
        }

        private void SetConnectionType()
        {
            Console.WriteLine("Starting a TCP connection");
            cxn  = ConnectionType.TCP;
 
        }

        private void SetupRPC()
        {
            Console.WriteLine("Setting up RPC - Private Client Object Instances");
            RemoteProcedureCalls.Server.RegisterTypeForPrivateRemoteCall<MagicEightBall, IMagicEightBall>();
            IPEndPoint IPE = IPTools.ParseEndPointFromString("127.0.0.1:19504");
            TCPConnection.StartListening(IPE);
        }
    }
}
