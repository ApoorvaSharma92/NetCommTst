using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProtoBuf;
using DPSBase;
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

            // Set application wide default for Send Receive Options (Serializer, encryption engine, compression engine)
            // We are setting it here to use Protocol Buffers as serialization engine and no compression or encryption engines.
            // Turning compression / encryption on results in a 95% reduction in throughput.  For instance on our test machine
            // RPC transactions (Round trips) went from 43 / second to 1500/second.  
            SendReceiveOptions nullCompressionSRO = new SendReceiveOptions(DPSManager.GetDataSerializer<ProtobufSerializer>(),null,null);
            NetworkComms.DefaultSendReceiveOptions = nullCompressionSRO;
            
 
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
