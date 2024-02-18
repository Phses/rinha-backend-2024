namespace RinhaBackendApi;

public static class Queries
{
    public const string CheckSeClienteExiste = @"SELECT COUNT(*) 
                                                 FROM Cliente 
                                                 WHERE Id = (@clienteId)";

    public const string UpdateSaldoCliente = @"UPDATE Cliente
                                               SET Saldo = Saldo + @sum
                                               WHERE Id = @ClienteId AND (@sum > 0 OR Saldo + @sum >= Limite * -1)";

    public const string InsereTransacao = @"INSERT INTO Transacao (
                                                ClienteId, 
                                                Valor,
                                                Tipo,
                                                Descricao) 
                                           VALUES (
                                                (@clienteId), 
                                                (@valor),                                                
                                                (@tipo),
                                                (@descricao)
                                            )";

    public const string GetDadosCliente = @"SELECT Limite, Saldo 
                                            FROM Cliente WHERE Id = @clienteId";

    public const string GetSaldo = @"SELECT Limite, NOW() as DataExtrato, Saldo 
                                     FROM Cliente WHERE Id = @clienteId";

    public const string GetTransacoes = @"SELECT t.Valor, t.Tipo, t.Descricao, t.RealizadaEm 
                                            FROM Transacao t WHERE ClienteId = @clienteId
                                            ORDER BY t.realizadaEm
                                            LIMIT 10";

}
