using System.Threading.Tasks;

namespace Sample.Functions.Accounts
{
    public interface IAccountEntity
    {
        Task Create(CreateAccountMessage message);
        Task Debit(DebitAccountMessage message);
        Task Credit(CreditAccountMessage message);
    }
}
