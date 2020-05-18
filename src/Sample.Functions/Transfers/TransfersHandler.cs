using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Sample.Functions.Accounts;
using System.Threading.Tasks;

namespace Sample.Functions.Transfers
{
    public class TransfersHandler
    {
        [FunctionName(nameof(ExecuteTransfer))]
        public async Task ExecuteTransfer([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var message = context.GetInput<TransferMessage>();

            var fromAccountEntity = new EntityId(nameof(AccountEntity), message.FromAccountId);
            var toAccountEntity = new EntityId(nameof(AccountEntity), message.ToAccountId);

            using (await context.LockAsync(fromAccountEntity, toAccountEntity))
            {
                var fromAccountProxy = context.CreateEntityProxy<IAccountEntity>(fromAccountEntity);
                var toAccountProxy = context.CreateEntityProxy<IAccountEntity>(toAccountEntity);

                await fromAccountProxy.Debit(new DebitAccountMessage(message.Amount));
                await toAccountProxy.Credit(new CreditAccountMessage(message.Amount));
            }
        }
    }
}
