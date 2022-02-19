using Npgsql;

using System.Data;

namespace CRUD_Cards_webapi.Dapper;

public class CardsDapperDbContext : IAsyncDisposable
{
    private readonly IDbConnection _connection;
    private bool _disposed;

    public CardsDapperDbContext(string connectionString)
    {
        _connection = new NpgsqlConnection(connectionString);
        _connection.Open();
    }

    public IDbConnection Connection
    {
        get
        {
            if (_disposed) throw new InvalidOperationException("Connection already disposed");
            return _connection;
        }
    }

    public ValueTask DisposeAsync()
    {
        _disposed = true;
        _connection.Close();
        Connection.Dispose();
        return ValueTask.CompletedTask;
    }
}