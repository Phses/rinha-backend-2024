using Npgsql;

namespace RinhaBackendApi;

public class TransactionDb(NpgsqlDataSource _dataSource)
{
    public async Task<Result<TransacaoResp>> Add(TransacaoReq transacao, int clienteId)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        
        var existe = await CheckSeClienteExiste(connection, clienteId);

        if(!existe)
            return Result<TransacaoResp>.EntityNotFound();

        var atualizado = await UpdateClinte(connection, transacao, clienteId);

        if (!atualizado)
            return Result<TransacaoResp>.EntityNotProcessed();

        return await InsertTransacao(connection, transacao, clienteId);
    }

    private static async Task<bool> CheckSeClienteExiste(NpgsqlConnection connection, int clienteId)
    {
        await using var cmd = new NpgsqlCommand(Queries.CheckSeClienteExiste, connection);

        cmd.Parameters.AddWithValue("clienteId", clienteId);
        var resultObj = await cmd.ExecuteScalarAsync();

        long result = (resultObj as long?) ?? 0;
        return result == 1;
    }

    private static async Task<bool> UpdateClinte(NpgsqlConnection connection, TransacaoReq transacao, int clienteId)
    {
        var sum = transacao.Tipo == "d" ? transacao.Valor * -1 : transacao.Valor;

        await using var cmd = new NpgsqlCommand(Queries.UpdateSaldoCliente, connection);

        cmd.Parameters.AddWithValue("sum", sum);
        cmd.Parameters.AddWithValue("ClienteId", clienteId);


        var count = await cmd.ExecuteNonQueryAsync();

        return count > 0;
    }

    private static async Task<Result<TransacaoResp>> InsertTransacao(NpgsqlConnection connection, TransacaoReq transacao, int clienteId)
    {
        await using var cmd = new NpgsqlCommand(Queries.InsereTransacao, connection);

        cmd.Parameters.AddWithValue("clienteId", clienteId);
        cmd.Parameters.AddWithValue("valor", transacao.Valor);
        cmd.Parameters.AddWithValue("tipo", transacao.Tipo);
        cmd.Parameters.AddWithValue("descricao", transacao.Descricao);

        await cmd.ExecuteNonQueryAsync();

        await using var queryCmd = new NpgsqlCommand(Queries.GetDadosCliente, connection);
        queryCmd.Parameters.AddWithValue("clienteId", clienteId);
        using var reader = await queryCmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            int limite = reader.GetInt32(0);
            int saldo = reader.GetInt32(1);

            return new TransacaoResp(Limite: limite, Saldo: saldo);
        }

        return Result<TransacaoResp>.WithError();
    }
}
