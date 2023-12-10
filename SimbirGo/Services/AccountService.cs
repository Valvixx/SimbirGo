using SimbirGo.Models.Db;
using SimbirGo.Repositories.Interfaces;
using SimbirGo.Services.Interfaces;

namespace SimbirGo.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        this._accountRepository = accountRepository;
    }
    
    public async Task<int> CreateAccount(DbAccount account)
    {
        return await _accountRepository.CreateAccount(account);
    }

    public async Task<DbAccount?> GetAccountData(string email, string password)
    {
        return await _accountRepository.GetAccountData(email, password);
    }
}
