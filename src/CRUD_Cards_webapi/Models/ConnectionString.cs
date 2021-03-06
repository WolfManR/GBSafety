using System.Text;

namespace CRUD_Cards_webapi.Models;

public sealed class ConnectionString
{
    public string Host { get; init; }
    public int Port { get; init; } = -1;
    public string Database { get; init; }
    public string UserId { get; init; }
    public string Password { get; init; }

    public string BuildWithoutDatabase() => Build(false);
    public string BuildWithDatabase() => Build(true);

    private string Build(bool includeDatabase)
    {
        StringBuilder sb = new ();

        sb.Append($"Server={Host};");
        //if(Port != -1) sb.Append($"Port={Port};");
        if(includeDatabase) sb.Append($"Database={Database};");
        sb.Append($"User Id={UserId};");
        sb.Append($"Password={Password};");

        return sb.ToString();
    }
}

public static class ConfigurationExtensions
{
    public static ConnectionString GetConnectionStringBuilder(this IConfiguration configuration, string name)
    {
       return configuration.GetSection($"ConnectionStrings:{name}").Get<ConnectionString>();
    }
}