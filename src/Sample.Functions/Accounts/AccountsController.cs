using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sample.Functions.Accounts
{
    public class AccountsController
    {
        [FunctionName(nameof(PostAccount))]
        public async Task PostAccount(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "accounts/{accountId}")] HttpRequestMessage request,
            string accountId,
            [DurableClient] IDurableEntityClient client
            )
        {
            var account = await request.Content.ReadAsAsync<PostAccountRequest>();
            var message = new CreateAccountMessage(account.AccountNumber, account.Balance);
            await client.SignalEntityAsync<IAccountEntity>(accountId, account => account.Create(message));
        }

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
