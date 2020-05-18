namespace Sample.Functions.Accounts
{
    public class PostAccountRequest
    {
        public PostAccountRequest(string accountNumber, decimal balance)
        {
            AccountNumber = accountNumber;
            Balance = balance;
        }

        public string AccountNumber { get; }
        public decimal Balance { get; }
    }
}
