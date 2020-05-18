namespace Sample.Functions.Accounts
{
    public class CreateAccountMessage
    {
        public CreateAccountMessage(string accountNumber, decimal balance)
        {
            AccountNumber = accountNumber;
            Balance = balance;
        }

        public string AccountNumber { get; }
        public decimal Balance { get; }
    }
}
