#language: pt-br
Funcionalidade: Serviço de Arquivo
	ler arquivos em um determinado diretorio
	Gravar arquivos em um determinado diretorio
	Ler o conteudo de um arquivo
	Listar todos os arquivos
	Deletar um arquivo

@Fect()
Cenario: Escreve um arquivo
	Dado que tenho o diretório '.\data'
	E que tenho um texto 'Teste de gravação'
	Então o sistema deve ser capaz de gravar um arquivo nome: 'Teste' e extensão 'dat'

@Fect()
Cenario: Escreve mais um arquivo
	Dado que tenho o diretório '.\data'
	E que tenho um texto 'Teste de gravação 2'
	Então o sistema deve ser capaz de gravar um arquivo nome: 'Teste2' e extensão 'dat'

@Fect()
Cenário: Ler TODOS os arquivos
	Dado que tenho o diretório '.\data'
	E que tenho um texto 'Teste de gravação'
	Então o sistema deve ser capaz de gravar um arquivo nome: 'TesteTodos1' e extensão 'teste'
	Então o sistema deve ser capaz de gravar um arquivo nome: 'TesteTodos2' e extensão 'teste'
	E tenho 2 arquivos salvos

@Fect()
Cenario: Remover TODOS os arquivos
	Dado que tenho o diretório './data/todos'
	E que tenho 2 arquivos
	Então removo os dois
	E não encontro mais nenhum arquivo