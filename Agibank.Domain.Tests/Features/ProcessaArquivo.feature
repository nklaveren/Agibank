#language:pt-br
Funcionalidade: Processar Arquivo
	Dado um arquivo de entrada
	deve gerar um arquivo de saida

@ProcessarArquivoService
Cenário: Processar um arquivo
	Dado um arquivo 'teste' no diretorio './data/in' e com diretorio de saida './data/out'
	Então Gero um arquivo de saida com conteudo '2ç2ç10çPaulo'