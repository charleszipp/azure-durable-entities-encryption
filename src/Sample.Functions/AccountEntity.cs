using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using Sample.Functions.Encryption;
using System.Threading.Tasks;

namespace Sample.Functions
{
    [JsonObject(MemberSerialization = MemberSerialization.OptOut)]
    public class AccountEntity : IAccountEntity
    {
        [Encrypted]
        public string AccountNumber { get; set; }

        public void Set(string accountNumber) => AccountNumber = accountNumber;

        [FunctionName(nameof(AccountEntity))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx) => ctx.DispatchAsync<AccountEntity>();
    }
}
