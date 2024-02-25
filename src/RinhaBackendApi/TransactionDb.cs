using Npgsql;

namespace RinhaBackendApi;

public class TransactionDb
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly NpgsqlConnection _connection;
    public TransactionDb(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
        _connection = _dataSource.OpenConnection();
    }
    public async Task<Result<TransacaoResp>> Add(TransacaoReq transacao, int clienteId)
    {
        await _connection.OpenAsync();
        
        var existe = await CheckSeClienteExiste(clienteId);

        if(!existe)
            return Result<TransacaoResp>.EntityNotFound();

        var atualizado = await UpdateClinte(transacao, clienteId);

        if (!atualizado)
            return Result<TransacaoResp>.EntityNotProcessed();

        return await InsertTransacao(transacao, clienteId);
    }

    private async Task<bool> CheckSeClienteExiste(int clienteId)
    {
        await using var cmd = new NpgsqlCommand(Queries.CheckSeClienteExiste, _connection);

        cmd.Parameters.AddWithValue("clienteId", clienteId);
        var resultObj = await cmd.ExecuteScalarAsync();

        long result = (resultObj as long?) ?? 0;
        return result == 1;
    }

    private async Task<bool> UpdateClinte(TransacaoReq transacao, int clienteId)
    {
        var sum = transacao.Tipo == "d" ? transacao.Valor * -1 : transacao.Valor;

        await using var cmd = new NpgsqlCommand(Queries.UpdateSaldoCliente, _connection);

        cmd.Parameters.AddWithValue("sum", sum);
        cmd.Parameters.AddWithValue("ClienteId", clienteId);


        var count = await cmd.ExecuteNonQueryAsync();

        return count > 0;
    }

    private async Task<Result<TransacaoResp>> InsertTransacao(TransacaoReq transacao, int clienteId)
    {
        await using var cmd = new NpgsqlCommand(Queries.InsereTransacao, _connection);

        cmd.Parameters.AddWithValue("clienteId", clienteId);
        cmd.Parameters.AddWithValue("valor", transacao.Valor);
        cmd.Parameters.AddWithValue("tipo", transacao.Tipo);
        cmd.Parameters.AddWithValue("descricao", transacao.Descricao);

        await cmd.ExecuteNonQueryAsync();

        await using var queryCmd = new NpgsqlCommand(Queries.GetDadosCliente, _connection);
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
