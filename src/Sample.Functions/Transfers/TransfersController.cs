using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sample.Functions.Transfers
{
    public class TransfersController
    {
        [FunctionName(nameof(PostTransfer))]
        public async Task PostTransfer(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "transfers")] HttpRequestMessage request,
            [DurableClient] IDurableOrchestrationClient client
            )
        {
            var transfer = await request.Content.ReadAsAsync<PostTransferRequest>();
            var message = new TransferMessage(transfer.FromAccountId, transfer.ToAccountId, transfer.Amount);
            await client.StartNewAsync(nameof(TransfersHandler.ExecuteTransfer), message);
        }
    }
}
