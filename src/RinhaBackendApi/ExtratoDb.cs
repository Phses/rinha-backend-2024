using Npgsql;

namespace RinhaBackendApi;

public class ExtratoDb(NpgsqlDataSource _dataSource)
{
    public async Task<Result<Extrato>> Get(int clienteId)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        var existe = await CheckSeClienteExiste(connection, clienteId);

        if(!existe)
            return Result<Extrato>.EntityNotFound();

        var saldo = await GetSaldo(connection, clienteId);

        var transacoes = await GetTransacoes(connection, clienteId);

        return new Extrato(saldo, transacoes);
    }

    private static async Task<bool> CheckSeClienteExiste(NpgsqlConnection connection, int clienteId)
    {
        await using var cmd = new NpgsqlCommand(Queries.CheckSeClienteExiste, connection);

        cmd.Parameters.AddWithValue("clienteId", clienteId);
        var resultObj = await cmd.ExecuteScalarAsync();

        long result = (resultObj as long?) ?? 0;
        return result == 1;
    }

    private static async Task<Saldo?> GetSaldo(NpgsqlConnection connection, int clienteId)
    {
        await using var cmd = new NpgsqlCommand(Queries.GetSaldo, connection);
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

    private static async Task<List<Transacao>> GetTransacoes(NpgsqlConnection connection, int clienteId)
    {
        await using var cmd = new NpgsqlCommand(Queries.GetTransacoes, connection);
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
