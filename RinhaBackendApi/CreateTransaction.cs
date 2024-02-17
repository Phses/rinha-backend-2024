using Npgsql;

namespace RinhaBackendApi;

public class CreateTransaction(NpgsqlDataSource _dataSource)
{
    private const string connectionString = "Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase";

    public CreateTransaction() : this(NpgsqlDataSource.Create(connectionString))
    {
    }
    public async Task Add(TransacaoReq transacao, int clienteId)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        string commandText = $"INSERT INTO Transacao (ClientId, Valor, Descricao, RealizadaEm) VALUES (@clientId, @valor, @descricao, @realizadaEm)";
        await using var command = new NpgsqlCommand(commandText, connection);

        using var cmd = new NpgsqlCommand(commandText, connection);

        cmd.Parameters.AddWithValue("clientId", clienteId);
        cmd.Parameters.AddWithValue("valor", transacao.Valor);
        cmd.Parameters.AddWithValue("descricao", transacao.Descricao);
        cmd.Parameters.AddWithValue("realizadaEm", DateTime.UtcNow);

        await command.ExecuteNonQueryAsync();
    }
}
