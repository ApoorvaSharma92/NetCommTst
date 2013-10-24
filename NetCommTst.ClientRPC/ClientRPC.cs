using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCommTst.ClientRPC
{
    class ClientRPC
    {
        static void Main(string[] args)
        {
            ClientInstance CI = new ClientInstance();
            CI.Start();
            Console.ReadLine();
        }
    }
}
