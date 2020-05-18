namespace Sample.Functions.Accounts
{
    public class DebitAccountMessage
    {
        public DebitAccountMessage(decimal amount)
        {
            Amount = amount;
        }

        public decimal Amount { get; }
    }
}
