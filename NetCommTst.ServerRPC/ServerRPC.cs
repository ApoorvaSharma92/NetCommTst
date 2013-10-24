using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetworkCommsDotNet;

namespace NetCommTst.ServerRPC
{
    class ServerRPC
    {
        static void Main(string[] args)
        {
            ServerInstance SI = new ServerInstance();
            SI.Start();
            Console.WriteLine("Server is waiting for connections");
            Console.ReadLine();
        }
    }
}
