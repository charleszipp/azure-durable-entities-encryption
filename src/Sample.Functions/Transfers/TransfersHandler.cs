using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Sample.Functions.Accounts;
using System.Threading.Tasks;

namespace Sample.Functions.Transfers
{
    public class TransfersHandler
    {
        /* TODO: Write tests to exercise the following scenarios
 
         Scenario: Transfer
	        Given account 1 with number 111 has balance of $100
	        and account 2 with number 222 has balance of $50
	        When transfer is made for $3 from account 1 to account 2
	        Then account 1 should have a balance of $97
	        and account 2 should have a balance of $53

        Scenario: Transfer more than available balance
	        Given account 1 with number 111 has balance of $100
	        and account 2 with number 222 has balance of $50
	        When transfer is made for $200 from account 1 to account 2
	        Then the transfer should be rejected due to overdraft
         
        */
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
