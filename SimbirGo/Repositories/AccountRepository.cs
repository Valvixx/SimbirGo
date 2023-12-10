using System.Data.Common;
using Dapper;
using SimbirGo.Data;
using SimbirGo.Models.Db;
using SimbirGo.Repositories.Interfaces;

namespace SimbirGo.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly DbConnection _dbConnection;

    public AccountRepository(DbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<bool> CheckAccount(string email)
    {
        return await _dbConnection.QueryFirstOrDefaultAsync<DbAccount>(
            @$"select id from accounts where email = '{email}'") != null;
    }

    [Obsolete("Obsolete")]
    public async Task<int> CreateAccount(DbAccount user)
    {
        var id = await _dbConnection.ExecuteAsync(
            @$"insert into accounts (email, password, username, role, refresh_token, refresh_token_expired_time)
                values ('{user.Email}',
                        '{HashGenerator.GetHash(user.Password)}', 
                        '{user.Username}', 
                        '{user.Role}',
                        '{user.RefreshToken}', 
                        '{user.RefreshTokenExpiredTime}')
                         returning id");

        return id;
    }

    [Obsolete("Obsolete")]
    public async Task<DbAccount?> GetAccountData(string email, string password)
    {
        return await _dbConnection.QueryFirstOrDefaultAsync<DbAccount>(
            @$"select id, role, refresh_token as RefreshToken, username 
                from accounts where email = '{email}' and 
                password='{HashGenerator.GetHash(password)}'");
    }

    public async Task<bool> CheckRefreshToken(string token)
    {
        return await _dbConnection.QueryFirstOrDefaultAsync<string>(
            $@"select refresh_token from accounts where refresh_token = '{token}'") == null;
    }

    public async Task<DbAccount?> GetAccountDataByToken(string token)
    {
        return await _dbConnection.QueryFirstOrDefaultAsync<DbAccount>(
            $@"select id, email, username, role from accounts where refresh_token = '{token}'");
    }

    public async Task UpdateRefresh(string token, DateTime tokenTime)
    {
        await _dbConnection.ExecuteAsync($@"update accounts set 
                    refresh_token = '{token}', refresh_token_expired_time='{tokenTime}'");
    }
}