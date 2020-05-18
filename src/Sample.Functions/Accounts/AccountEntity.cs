using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using Sample.Functions.Encryption;
using System;
using System.Threading.Tasks;

namespace Sample.Functions.Accounts
{
    [JsonObject(MemberSerialization = MemberSerialization.OptOut)]
    public class AccountEntity : IAccountEntity
    {
        [Encrypted]
        public string AccountNumber { get; set; }

        public decimal Balance { get; set; }

        public Task Create(CreateAccountMessage message)
        {
            AccountNumber = message.AccountNumber;
            Balance = message.Balance;
            return Task.CompletedTask;
        }

        public Task Debit(DebitAccountMessage message)
        {
            Balance -= Math.Abs(message.Amount);
            return Task.CompletedTask;
        }

        public Task Credit(CreditAccountMessage message)
        {
            Balance += Math.Abs(message.Amount);
            return Task.CompletedTask;
        }

        [FunctionName(nameof(AccountEntity))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx) 
            => ctx.DispatchAsync<AccountEntity>();
    }
}
