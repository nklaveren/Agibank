#language: pt-br
Funcionalidade: Análise Vendas
o sistema deve importar
lotes de arquivos, ler e analisar os dados e produzir um relatório.
Existem 3 tipos de dados dentro desses arquivos.
Para cada tipo de dados há um layout diferente.

@(FECT)
Cenário: Dados do vendedor
	Dado que tenho dados do vendedor
		| Linha                         |
		| 001ç1234567891234çPedroç50000 |
	Então O resultado esperado é uma instancia de um vendedor:
		| Tipo | CPF           | Nome  | Salario |
		| 001  | 1234567891234 | Pedro | 50000   |

@(FECT)
Cenário: Dados do Cliente
	Dado que tenho dados do Cliente
		| Linha                                    |
		| 002ç2345675434544345çJose da SilvaçRural |
	Então O resultado esperado é uma instancia de um Cliente:
		| Tipo | CNPJ             | Nome          | Area Negocio |
		| 002  | 2345675434544345 | Jose da Silva | Rural        |

@(FECT)
Cenário: Dados da venda
	Dado que tenho dados da venda
		| linha                                       |
		| 003ç10ç[1-10-100,2-30-2.50,3-40-3.10]çPedro |
	Então O resultado esperado é uma instancia de uma Venda:
		| Tipo | Venda Id | Item                           | Vendedor Nome |
		| 003  | 10       | [1-10-100,2-30-2.50,3-40-3.10] | Pedro         |
	E os itens da da venda são:
		| Item ID | Item Quantidade | Item Preço |
		| 1       | 10              | 100        |
		| 2       | 30              | 2.50       |
		| 3       | 40              | 3.10       |

@(FECT)
Cenário: Relatório completo
	Dado que tenho as informações:
		| Linhas                                      |
		| 001ç1234567891234çPedroç50000               |
		| 001ç3245678865434çPauloç40000.99            |
		| 002ç2345675434544345çJose da SilvaçRural    |
		| 002ç2345675433444345çEduardo PereiraçRural  |
		| 003ç10ç[1-10-100,2-30-2.50,3-40-3.10]çPedro |
		| 003ç08ç[1-34-10,2-33-1.50,3-40-0.10]çPaulo  |
	Então O resultado esperado da analise é:
		| Quantidade Clientes | Quantidade Vendededor | Venda mais cara | Pior Vendedor |
		| 2                   | 2                     | 10              | Paulo         |