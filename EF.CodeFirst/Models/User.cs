namespace EF.CodeFirst.Models;

public class User
{
    public int Id { get; set; }
    public string UserIp { get; set; }
    public int SearchesLeft { get; set; }
}