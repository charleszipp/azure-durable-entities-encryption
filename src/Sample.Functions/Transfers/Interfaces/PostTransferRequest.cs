namespace Sample.Functions.Transfers
{
    public class PostTransferRequest
    {
        public PostTransferRequest(string fromAccountId, string toAccountId, decimal amount)
        {
            FromAccountId = fromAccountId;
            ToAccountId = toAccountId;
            Amount = amount;
        }

        public string FromAccountId { get; }
        public string ToAccountId { get; }
        public decimal Amount { get; }
    }
}
