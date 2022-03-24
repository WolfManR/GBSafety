﻿using Npgsql;

using System.Data;
using Dapper;

namespace CRUD_Cards_webapi.Dapper;

public class CardsDapperDbContext : IAsyncDisposable
{
    private readonly NpgsqlConnection _connection;
    private bool _disposed;

    public CardsDapperDbContext(string connectionString, string database)
    {
        _connection = new NpgsqlConnection(connectionString);
        _connection.Open();
        try
        {
            _connection.ChangeDatabase(database);
        }
        catch (PostgresException e)
        {
            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = $"CREATE DATABASE \"{database}\"";
            command.ExecuteNonQuery();
            _connection.ChangeDatabase(database);
        }
    }

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
        _connection.Dispose();
        return ValueTask.CompletedTask;
    }
}