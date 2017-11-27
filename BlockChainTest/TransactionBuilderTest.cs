using System;
using System.Collections.Generic;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using HiddenWallet.WebClients.SmartBit;
using System.Threading;

namespace BlockChainTest
{
    class TransactioBuilderTest
    {

        static void Main(string[] args)
        {



            //Carrying out a Transaction

            var bitcoinPrivateKey = new BitcoinSecret("cNZupUgfs54DmsShwxa1wpomQcszUtuJYFvx9zWPbXrT7KsWtiUd");
            var network = bitcoinPrivateKey.Network;
            var address = bitcoinPrivateKey.GetAddress();
            var client = new QBitNinjaClient(network);
            var balance = client.GetBalance(address, true).Result;

            var transactionId = uint256.Parse("3f3278d550ecd4f921d7f6f48f1e4b873e39d4b1b2a5098867d4c4e30ab9b444");
            var transactionResponse = client.GetTransaction(transactionId).Result;


            var chimaTestDestinationAddress = new BitcoinPubKeyAddress("mxgN2AiqHjKfGvo6Y57fAe4Y754rPdKf4P");

            var ugoCoins = new HashSet<ICoin>();
            foreach(var operation in balance.Operations)
            {
                foreach(var coin in operation.ReceivedCoins)
                {
                    if(coin.TxOut.ScriptPubKey.GetDestinationAddress(network) == address)
                    {
                        ugoCoins.Add(coin);
                    }
                }
            }

            var txBuilder = new TransactionBuilder();
            var trans = txBuilder
                .AddCoins(ugoCoins)
                .AddKeys(bitcoinPrivateKey)
                .Send(chimaTestDestinationAddress.ScriptPubKey.GetDestination(), "0.10")
                .SendFees("0.01")
                .SetChange(bitcoinPrivateKey.GetAddress())
                .BuildTransaction(true);
            bool isSigned = txBuilder.Verify(trans);
            
            SmartBitClient smartBitClient = new SmartBitClient(network);
            PushTransaction(trans);

            //HttpResponseMessage response =  httpClient.PostAsync(new SmartBit() { hex = ""}).Result;

            BroadcastResponse broadcast = client.Broadcast(trans).Result;
            if (!broadcast.Success)
            {
                Console.WriteLine("ErrorCode: " + broadcast.Error.ErrorCode);
                Console.WriteLine("Error message: " + broadcast.Error.Reason);
            }
            else
            {
                Console.WriteLine("Success, you can now checkout the transaction in any block explorer");
                Console.WriteLine("Hash: " + trans.GetHash());
            }


            Console.ReadKey();
        }

        public static void PushTransaction(Transaction transaction)
        {
            SmartBit smartBit = new SmartBit();
            smartBit.hex = transaction.ToHex();
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://testnet-api.smartbit.com.au/v1/");

            HttpResponseMessage response =
                     httpClient.PostAsJsonAsync("blockchain/pushtx", smartBit).Result;


            SmartBit smartBitResponse = response.Content.ReadAsAsync<SmartBit>().Result;

        }
    }
}
