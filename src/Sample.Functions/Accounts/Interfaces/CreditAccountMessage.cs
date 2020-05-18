namespace Sample.Functions.Accounts
{
    public class CreditAccountMessage
    {
        public CreditAccountMessage(decimal amount)
        {
            Amount = amount;
        }

        public decimal Amount { get; }
    }
}
