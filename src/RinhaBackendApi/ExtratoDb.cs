using Npgsql;

namespace RinhaBackendApi;

public class ExtratoDb
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly NpgsqlConnection _connection;
    public ExtratoDb(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
        _connection = _dataSource.OpenConnection();
    }
    public async Task<Result<Extrato>> Get(int clienteId)
    {
        await _connection.OpenAsync();

        var existe = await CheckSeClienteExiste(clienteId);

        if(!existe)
            return Result<Extrato>.EntityNotFound();

        var saldo = await GetSaldo(clienteId);

        var transacoes = await GetTransacoes(clienteId);

        return new Extrato(saldo, transacoes);
    }

    private async Task<bool> CheckSeClienteExiste(int clienteId)
    {
        await using var cmd = new NpgsqlCommand(Queries.CheckSeClienteExiste, _connection);

        cmd.Parameters.AddWithValue("clienteId", clienteId);
        var resultObj = await cmd.ExecuteScalarAsync();

        long result = (resultObj as long?) ?? 0;
        return result == 1;
    }

    private async Task<Saldo?> GetSaldo(int clienteId)
    {
        await using var cmd = new NpgsqlCommand(Queries.GetSaldo, _connection);
        cmd.Parameters.AddWithValue("clienteId", clienteId);
        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            int limite = reader.GetInt32(0);
            DateTime dataExtrato = reader.GetDateTime(1);
            int saldo = reader.GetInt32(2);

            return new Saldo(Limite: limite, DataExtrato: dataExtrato, Total: saldo);
        }

        return null;
    }

    private async Task<List<Transacao>> GetTransacoes(int clienteId)
    {
        await using var cmd = new NpgsqlCommand(Queries.GetTransacoes, _connection);
        cmd.Parameters.AddWithValue("clienteId", clienteId);
        using var reader = await cmd.ExecuteReaderAsync();
        
        var transacoes = new List<Transacao>();

        while (await reader.ReadAsync())
        {
           transacoes.Add(new Transacao(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3)));
        }

        return transacoes;
    }
}
