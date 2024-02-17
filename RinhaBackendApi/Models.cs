namespace RinhaBackendApi;

//Todo: criar enum c para credito e d para debito
public record TransacaoReq(int Valor, string Tipo, string Descricao);

public record TransacaoResp(int Limite, int Saldo);

public record Extrato(Saldo Saldo, List<Transacao> Transacoes);

public record Saldo(int Total, DateTime DataExtrato, int Limite);

public record Transacao(int Valor, string Tipo, string Descricao, DateTime RealizadaEm);