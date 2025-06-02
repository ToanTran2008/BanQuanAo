using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using System.Threading.Tasks;

namespace DOAN.Controllers
{
    public class EthereumService
    {
        private readonly string _contractAddress = "0x6eaB2060fc2324BAD065401971953b1eE9Bb3E9C"; // Địa chỉ hợp đồng đã deploy
        private readonly string _rpcUrl = "http://127.0.0.1:7545"; // Ganache RPC

        // Thông tin ví test (LẤY TỪ GANACHE)
        private readonly string _privateKey = "YOUR_PRIVATE_KEY_HERE"; // ví dụ: 0xabcdef123...
        private readonly string _accountAddress = "0xYourAccountHere"; // ví dụ: 0xAbc123...

        private readonly string abi = @"[
          {
            ""inputs"": [
              { ""internalType"": ""string"", ""name"": ""_email"", ""type"": ""string"" },
              { ""internalType"": ""uint256"", ""name"": ""_amount"", ""type"": ""uint256"" },
              { ""internalType"": ""string"", ""name"": ""_paymentMethod"", ""type"": ""string"" }
            ],
            ""name"": ""addOrder"",
            ""outputs"": [],
            ""stateMutability"": ""nonpayable"",
            ""type"": ""function""
          }
        ]";

        public async Task<string> AddOrderToBlockchain(string buyerEmail, int totalAmount, string paymentMethod)
        {
            // Tạo tài khoản có private key
            var account = new Account(_privateKey);
            var web3 = new Web3(account, _rpcUrl);

            var contract = web3.Eth.GetContract(abi, _contractAddress);
            var function = contract.GetFunction("addOrder");

            // Gửi giao dịch
            var txHash = await function.SendTransactionAsync(
                from: _accountAddress,
                gas: new HexBigInteger(3000000),
                value: new HexBigInteger(0),
                functionInput: new object[] { buyerEmail, totalAmount, paymentMethod }
            );

            return txHash;
        }
    }
}
