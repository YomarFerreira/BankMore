### contacorrente

CREATE TABLE contacorrente (
	idcontacorrente TEXT(37) PRIMARY KEY, -- id da conta corrente
	numero INTEGER(10) NOT NULL UNIQUE, -- numero da conta corrente
	nome TEXT(100) NOT NULL, -- nome do titular da conta corrente
	ativo INTEGER(1) NOT NULL default 0, -- indicativo se a conta esta ativa. (0 = inativa, 1 = ativa).
	senha TEXT(100) NOT NULL,
	salt TEXT(100) NOT NULL,
	CHECK (ativo in (0,1))
);

CREATE TABLE movimento (
	idmovimento TEXT(37) PRIMARY KEY, -- identificacao unica do movimento
	idcontacorrente TEXT(37) NOT NULL, -- identificacao unica da conta corrente
	datamovimento TEXT(25) NOT NULL, -- data do movimento no formato DD/MM/YYYY
	tipomovimento TEXT(1) NOT NULL, -- tipo do movimento. (C = Credito, D = Debito).
	valor REAL NOT NULL, -- valor do movimento. Usar duas casas decimais.
	CHECK (tipomovimento in ('C','D')),
	FOREIGN KEY(idcontacorrente) REFERENCES contacorrente(idcontacorrente)
);

CREATE TABLE idempotencia (
	chave_idempotencia TEXT(37) PRIMARY KEY, -- identificacao chave de idempotencia
	requisicao TEXT(1000), -- dados de requisicao
	resultado TEXT(1000) -- dados de retorno
);


### transferencia

CREATE TABLE transferencia (
	idtransferencia TEXT(37) PRIMARY KEY, -- identificacao unica da transferencia
	idcontacorrente_origem TEXT(37) NOT NULL, -- identificacao unica da conta corrente de origem
	idcontacorrente_destino TEXT(37) NOT NULL, -- identificacao unica da conta corrente de destino
	datamovimento TEXT(25) NOT NULL, -- data do transferencia no formato DD/MM/YYYY
	valor REAL NOT NULL, -- valor da transferencia. Usar duas casas decimais.
	FOREIGN KEY(idtransferencia) REFERENCES transferencia(idtransferencia)
);

CREATE TABLE idempotencia (
	chave_idempotencia TEXT(37) PRIMARY KEY, -- identificacao chave de idempotencia
	requisicao TEXT(1000), -- dados de requisicao
	resultado TEXT(1000) -- dados de retorno
);


### tarifa.sql

CREATE TABLE tarifa (
	idtarifa TEXT(37) PRIMARY KEY, -- identificacao unica da tarifa
	idcontacorrente TEXT(37) NOT NULL, -- identificacao unica da conta corrente
	datamovimento TEXT(25) NOT NULL, -- data do transferencia no formato DD/MM/YYYY
	valor REAL NOT NULL, -- valor da tarifa. Usar duas casas decimais.
	FOREIGN KEY(idtarifa) REFERENCES tarifa(idtarifa)
);