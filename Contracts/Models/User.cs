namespace Contracts.Models;

public class User
{
    public string UserIp { get; set; }
    public int SearchesLeft { get; set; }

    public User(string userIp, int searchesLeft)
    {
        UserIp = userIp;
        SearchesLeft = searchesLeft;
    }
}