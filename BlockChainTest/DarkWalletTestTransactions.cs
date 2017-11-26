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
    class DarkWalletTestTransactions
    {
        static void Test(string[] args)
        {



            //Carrying out a Transaction
            
            var bitcoinPrivateKey = new BitcoinSecret("cNZupUgfs54DmsShwxa1wpomQcszUtuJYFvx9zWPbXrT7KsWtiUd");
            var network = bitcoinPrivateKey.Network;
            var address = bitcoinPrivateKey.GetAddress();
            var client = new QBitNinjaClient(network);
            var balance = client.GetBalance(address,true).Result;

            var transactionId = uint256.Parse("3f3278d550ecd4f921d7f6f48f1e4b873e39d4b1b2a5098867d4c4e30ab9b444");
            var transactionResponse = client.GetTransaction(transactionId).Result;

            var tx = new Transaction();
            foreach (var operation in balance.Operations)
            {
                OutPoint spendOutPoint = null;
                
                var coinsReceived = operation.ReceivedCoins;
                foreach (var coin in coinsReceived)
                {
                    if (coin.TxOut.ScriptPubKey == bitcoinPrivateKey.ScriptPubKey)
                    {
                        spendOutPoint = coin.Outpoint;
                        tx.Inputs.Add(new TxIn()
                        {
                            PrevOut = spendOutPoint
                        });
                    }
                }
                

            }
            var chimaTestDestinationAddress = new BitcoinPubKeyAddress("mxgN2AiqHjKfGvo6Y57fAe4Y754rPdKf4P");
            TxOut chimaTestDestinationAddressTxOut = new TxOut()
            {
                Value = new Money((decimal)0.50, MoneyUnit.BTC),
                ScriptPubKey = chimaTestDestinationAddress.ScriptPubKey
            };
            TxOut ugoChangeBackTxOut = new TxOut()
            {
                Value = new Money((decimal)2.39, MoneyUnit.BTC),
                ScriptPubKey = bitcoinPrivateKey.ScriptPubKey
            };
            tx.Outputs.Add(chimaTestDestinationAddressTxOut);
            tx.Outputs.Add(ugoChangeBackTxOut);
            var msg = "ugo the jedi master";
            var msgBytes = Encoding.UTF8.GetBytes(msg);

            TxOut txDesc = new TxOut()
            {
                Value = Money.Zero,
                ScriptPubKey = TxNullDataTemplate.Instance.GenerateScriptPubKey(msgBytes)
            };
            tx.Outputs.Add(txDesc);


            tx.Inputs[0].ScriptSig = bitcoinPrivateKey.PubKey.ScriptPubKey;
            tx.Inputs[1].ScriptSig = bitcoinPrivateKey.PubKey.ScriptPubKey;
            tx.Inputs[2].ScriptSig = bitcoinPrivateKey.PubKey.ScriptPubKey;
            tx.Sign(bitcoinPrivateKey, false);
            string txHex = tx.ToHex();

            BroadcastResponse broadcast = client.Broadcast(tx).Result;
            if (!broadcast.Success)
            {
                Console.WriteLine("ErrorCode: " + broadcast.Error.ErrorCode);
                Console.WriteLine("Error message: " + broadcast.Error.Reason);
            }
            else
            {
                Console.WriteLine("Success, you can now checkout the transaction in any block explorer");
                Console.WriteLine("Hash: " + tx.GetHash());
            }

            //end of our dark wallet
  


            Console.ReadKey();
        }
    }
}
