namespace SimbirGo.Models;

public class AccountModel
{
    public AccountModel(string username, string password)
    {
        this.Username = username;
        this.Password = password;
    }

    public string Username { get; set; }
    public string Password { get; set; }
}