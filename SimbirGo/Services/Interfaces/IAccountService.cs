using System.Security.Claims;
using SimbirGo.Models.Db;

namespace SimbirGo.Services.Interfaces;

public interface IAccountService
{
    Task<int> CreateAccount(DbAccount account);
    Task<DbAccount?> GetAccountData(string email, string password);
}