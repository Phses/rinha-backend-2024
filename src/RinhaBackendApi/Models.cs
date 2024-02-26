namespace RinhaBackendApi;

//Todo: criar enum c para credito e d para debito
public record TransacaoReq(int valor, string tipo, string descricao);

public record TransacaoResp(int limite, int saldo);

public record Extrato(Saldo saldo, List<Transacao> transacoes);

public record Saldo(int total, DateTime data_extrato, int limite);

public record Transacao(int valor, string tipo, string descricao, DateTime realizada_em);