using System;
using System.Collections.Generic;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System.Diagnostics;
using System.Text;

namespace BlockChainTest
{
    class Program
    {
        static void Test(string[] args)
        {
            var qBitNinjaClient = new QBitNinjaClient(Network.TestNet);
            var destAddress = new BitcoinPubKeyAddress("ms2aSazNDyGdjSJcQKoottewTw4eY2yPRZ");
            var balance = qBitNinjaClient.GetBalance(destAddress).Result;

            //var network = Network.Main;
            //var privateKey = new Key();
            //var bitcoinPrivateKey = privateKey.GetWif(Network.Main);
            //var address = bitcoinPrivateKey.GetAddress();
            //Console.WriteLine(bitcoinPrivateKey);
            //Console.WriteLine(address);

            //Carrying out a Transaction
            var bitcoinPrivateKey = new BitcoinSecret("L46GfzE4z24gmX7d3MTWh2dRjTpd6jv4SB3zTB88uvFniMwTbg4B");
            var network = bitcoinPrivateKey.Network;
            var address = bitcoinPrivateKey.GetAddress();
            var client = new QBitNinjaClient(network);
            var transactionId = uint256.Parse("06c0aec7543467951abad0c28998a2c1fc1cdc34e01113f8ec1fdb22be854771");
            var transactionResponse = client.GetTransaction(transactionId).Result;
            
            
            

            Console.WriteLine(transactionResponse.TransactionId);
            if (transactionResponse.Block != null)
            {
                Console.WriteLine(transactionResponse.Block.Confirmations);
            }
            
            var receivedCoins = transactionResponse.ReceivedCoins;
            OutPoint outPointToSpend = null;
            foreach (var coin in receivedCoins)
            {
                if (coin.TxOut.ScriptPubKey == bitcoinPrivateKey.ScriptPubKey)
                {
                    outPointToSpend = coin.Outpoint;
                }
            }

            var transaction = new Transaction();
            transaction.Inputs.Add(new TxIn()
            {
                PrevOut = outPointToSpend
            });

            var ugoBtcDestinationAddress = new BitcoinPubKeyAddress("15iDEceRgbfvAGdsCHSb2stUR3tSdxVU35");
            TxOut ugoBtcDestinationAddressTxOut = new TxOut()
            {
                Value = new Money((decimal)0.00034492, MoneyUnit.BTC),
                ScriptPubKey = ugoBtcDestinationAddress.ScriptPubKey
            };
            TxOut changeBackTxOut = new TxOut()
            {
                Value = new Money((decimal)0.00017246, MoneyUnit.BTC),
                ScriptPubKey = bitcoinPrivateKey.ScriptPubKey
            };
            transaction.Outputs.Add(ugoBtcDestinationAddressTxOut);
            transaction.Outputs.Add(changeBackTxOut);
            var message = "ugo the jedi master";
            var bytes = Encoding.UTF8.GetBytes(message);

            TxOut transactionDescription = new TxOut()
            {
                Value = Money.Zero,
                ScriptPubKey = TxNullDataTemplate.Instance.GenerateScriptPubKey(bytes)
            };
            transaction.Outputs.Add(transactionDescription);

            transaction.Inputs[0].ScriptSig = bitcoinPrivateKey.ScriptPubKey;
            transaction.Sign(bitcoinPrivateKey, false);

            BroadcastResponse broadcastResponse = client.Broadcast(transaction).Result;
            if (!broadcastResponse.Success)
            {
                Console.WriteLine("ErrorCode: " + broadcastResponse.Error.ErrorCode);
                Console.WriteLine("Error message: " + broadcastResponse.Error.Reason);
            }
            else
            {
                Console.WriteLine("Success, you can now checkout the transaction in any block explorer");
                Console.WriteLine("Hash: " + transaction.GetHash());
            }



            Console.ReadKey();
        }
    }
}
