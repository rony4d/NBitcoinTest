
using System;
using System.Collections.Generic;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System.Diagnostics;
using System.Text;
namespace BlockChainTest
{
    class ProofOfOwnership
    {
        static void Test(string[] args)
        {

 

            var bitcoinPrivateKey = new BitcoinSecret("L46GfzE4z24gmX7d3MTWh2dRjTpd6jv4SB3zTB88uvFniMwTbg4B");
            var message = "I am ugo the jedi master";
            string signature = bitcoinPrivateKey.PrivateKey.SignMessage(message);
            Console.WriteLine(signature);

            var address = new BitcoinPubKeyAddress("16LyFzae8r9a1tny8n66gCdrLXUPPcaRm8");
            bool isUgoTheJediMaster = address.VerifyMessage(message, signature);
            Console.WriteLine(isUgoTheJediMaster);

            //verify nicholas doria is the owner of this address:
            var nicholasSignature = "H1jiXPzun3rXi0N9v9R5fAWrfEae9WPmlL5DJBj1eTStSvpKdRR8Io6/uT9tGH/3OnzG6ym5yytuWoA9ahkC3dQ=";
            var nicholasAddress = new BitcoinPubKeyAddress("1KF8kUVHK42XzgcmJF4Lxz4wcL5WDL97PB");
            var nicholasMessage = "Nicolas Dorier Book Funding Address";
            bool isNicholasAddress = nicholasAddress.VerifyMessage(nicholasMessage, nicholasSignature);

            Console.ReadKey();
        }
    }
}
