using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sample.Functions
{
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
