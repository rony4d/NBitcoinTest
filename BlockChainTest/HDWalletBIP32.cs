
using System;
using System.Collections.Generic;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System.Diagnostics;
using System.Text;
namespace BlockChainTest
{
    class HDWalletBIP32
    {
        static void Test(string [] args)
        {
            

            ExtKey masterKey = new ExtKey();
            Console.WriteLine("Master Key: " + masterKey.ToString(Network.Main));
            for (int i = 0; i < 7; i++)
            {
                ExtKey key = masterKey.Derive((uint)i);
                Console.WriteLine("Key " + i + " : " + key.ToString(Network.Main));
            }

            //Going from Key to ExtKey

            ExtKey extKey = new ExtKey();
            byte[] chainCode = extKey.ChainCode;
            Key privateKey = extKey.PrivateKey;

            ExtKey newExtKey = new ExtKey(privateKey, chainCode);

            //Allowing a third party generate public keys(addresses) without knowing your private key
            ExtPubKey masterPubKey = masterKey.Neuter();
            for (int i = 0; i < 5; i++)
            {
                ExtPubKey pubkey = masterPubKey.Derive((uint)i);
                Console.WriteLine("PubKey " + i + " : " + pubkey.ToString(Network.Main));
            }

            //Testing using one value
            masterKey = new ExtKey();
            masterPubKey = masterKey.Neuter();

            //Payment server generate pubKey1
            ExtPubKey pubKey1 = masterPubKey.Derive((uint)184757585);

            //You get the private key
            ExtKey key1 = masterKey.Derive((uint)184757585);

            //Check if it is legit
            Console.WriteLine("Generated address : " + pubKey1.PubKey.GetAddress(Network.Main));
            Console.WriteLine("Expected address : " + key1.PrivateKey.PubKey.GetAddress(Network.Main));

            //Parent Key and Child Keys
            ExtKey parentExtKey = new ExtKey();
            ExtPubKey parentExtPubKey = parentExtKey.Neuter();

            ExtKey childExtKey = parentExtKey.Derive(0);
            ExtKey childExtKey2 = parentExtKey.Derive(2);
            ExtKey child2ExtKey = parentExtKey.Derive(1).Derive(2);
            string childAddress = childExtKey.PrivateKey.PubKey.GetAddress(Network.Main).ToString();
            string child2Address = child2ExtKey.PrivateKey.PubKey.GetAddress(Network.Main).ToString();

            ExtKey parentKey1Recovered = childExtKey.GetParentExtKey(parentExtPubKey);
            ExtKey parentKey2Recovered = childExtKey2.GetParentExtKey(parentExtPubKey);



            //Mnemonic Code for HD Keys
            Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            ExtKey hdRoot = mnemonic.DeriveExtKey("my password");
            var mnemonicStr = mnemonic.ToString();
            Console.WriteLine(mnemonic);

            //If you have the mnemonic and the password you can recover the hdRoot Key
            mnemonic = new Mnemonic(mnemonicStr, Wordlist.English);
            hdRoot = mnemonic.DeriveExtKey("my password");

            Console.ReadKey();
        }
    }
}
