CREATE TABLE Cliente(
	Id SERIAL PRIMARY KEY,
	Limite INT NOT NULL,
	Saldo INT NOT NULL
);

CREATE TABLE Transacao(
	Id SERIAL PRIMARY KEY,
	ClienteId INT NOT NULL,
	Valor INT NOT NULL,
	Tipo CHAR(1) not null,
	Descricao VARCHAR(10) NOT NULL,
	RealizadaEm TIMESTAMP DEFAULT now(),

	FOREIGN KEY (ClienteId) REFERENCES Cliente (Id)
);

INSERT INTO Cliente(Limite, Saldo) 
VALUES 
    (100000, 0),
    (80000, 0),
    (1000000, 0),
    (10000000, 0),
    (500000, 0);