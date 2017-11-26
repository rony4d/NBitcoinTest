using System;
using System.Collections.Generic;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System.Diagnostics;
using System.Text;
using System.Linq;
namespace BlockChainTest
{
    class OtherTypesOfOwnership
    {
        static void Test(string[] args)
        {
            var key = new Key();
            Console.WriteLine(key.PubKey.WitHash.ScriptPubKey);
            Console.ReadKey();

        }
    }
}
