
using System;
using System.Collections.Generic;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System.Diagnostics;
using System.Text;
namespace BlockChainTest
{
    class KeyGenerationAndEncryption
    {
        static void Test(string[] args)
        {
            var privateKey = new Key();
            var bitcoinPrivateKey = privateKey.GetWif(Network.Main);
            Console.WriteLine(bitcoinPrivateKey);
            BitcoinEncryptedSecret encryptedBitcoinPrivateKey = bitcoinPrivateKey.Encrypt("password");
            Console.WriteLine(encryptedBitcoinPrivateKey);
            var decryptedBitcoinPrivateKey = encryptedBitcoinPrivateKey.GetSecret("password");
            Console.WriteLine(decryptedBitcoinPrivateKey);
            Console.ReadKey();

        }
    }
}
