using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using Sample.Functions.Encryption;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Functions
{
    public interface IAccountEntity
    {
        void Set(string accountNumber);
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptOut)]
    public class AccountEntity : IAccountEntity
    {
        [Encrypted]
        public string AccountNumber { get; set; }

        public void Set(string accountNumber) => AccountNumber = accountNumber;

        [FunctionName(nameof(AccountEntity))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx) => ctx.DispatchAsync<AccountEntity>();
    }

    public class AccountsController
    {
        [FunctionName(nameof(PutAccount))]
        public Task PutAccount(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "accounts/{accountId}")] HttpRequestMessage request,
            string accountId,
            [DurableClient] IDurableEntityClient client
            ) 
            => client.SignalEntityAsync<IAccountEntity>(accountId, account => account.Set("123ABC"));

        [FunctionName(nameof(GetAccount))]
        public async Task<IActionResult> GetAccount(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts/{accountId}")] HttpRequestMessage request,
            string accountId,
            [DurableClient] IDurableEntityClient client
            )
        {
            var entityStateResponse = await client.ReadEntityStateAsync<AccountEntity>(new EntityId(nameof(AccountEntity), accountId));
            
            return entityStateResponse.EntityExists ? (ActionResult)new OkObjectResult(entityStateResponse.EntityState) : new NotFoundResult();
        }
    }
}
