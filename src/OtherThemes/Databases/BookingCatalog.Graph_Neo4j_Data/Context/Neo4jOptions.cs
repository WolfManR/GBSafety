using Neo4j.Driver;

namespace BookingCatalog.Graph_Neo4j_Data.Context;

// ReSharper disable once InconsistentNaming
public class Neo4jOptions
{
    public string Uri { get; init; }
    public string Login { get; init; }
    public string Password { get; init; }

    public IAuthToken GenerateAuthTokens()
    {
        return AuthTokens.Basic(Login, Password);
    }
}