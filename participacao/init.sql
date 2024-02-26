CREATE TABLE cliente(
	id SERIAL PRIMARY KEY,
	limite INT NOT NULL,
	saldo INT NOT NULL
);

CREATE TABLE transacao(
	id SERIAL PRIMARY KEY,
	cliente_id INT NOT NULL,
	valor INT NOT NULL,
	tipo CHAR(1) not null,
	descricao VARCHAR(10) NOT NULL,
	realizada_em TIMESTAMP DEFAULT now(),

	FOREIGN KEY (cliente_id) REFERENCES cliente (Id)
);

INSERT INTO cliente(limite, saldo) 
VALUES 
    (100000, 0),
    (80000, 0),
    (1000000, 0),
    (10000000, 0),
    (500000, 0);