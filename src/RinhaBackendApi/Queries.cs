namespace RinhaBackendApi;

public static class Queries
{
    public const string CheckSeClienteExiste = @"SELECT COUNT(*) 
                                                 FROM cliente 
                                                 WHERE id = (@clienteId)";

    public const string UpdateSaldoCliente = @"UPDATE cliente
                                               SET saldo = saldo + @sum
                                               WHERE id = @ClienteId AND (@sum > 0 OR saldo + @sum >= limite * -1)";

    public const string InsereTransacao = @"INSERT INTO transacao (
                                                cliente_id, 
                                                valor,
                                                tipo,
                                                descricao) 
                                           VALUES (
                                                (@clienteId), 
                                                (@valor),                                                
                                                (@tipo),
                                                (@descricao)
                                            )";

    public const string GetDadosCliente = @"SELECT limite, saldo 
                                            FROM Cliente WHERE Id = @clienteId";

    public const string GetSaldo = @"SELECT limite, NOW() as data_extrato, saldo 
                                     FROM Cliente WHERE Id = @clienteId";

    public const string GetTransacoes = @"SELECT t.valor, t.tipo, t.descricao, t.realizada_em 
                                            FROM Transacao t WHERE t.cliente_id = @clienteId
                                            ORDER BY t.realizada_em
                                            LIMIT 10";

}
