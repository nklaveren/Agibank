# Analise de Dados
  projeto tem um Produtor de mensagem onde controla com o rabbitmq uma fila para processar dados
  
  o consumidor consome a fila do rabbitmq.
  
  a idéia central é conseguir escalar em diversos containers o consumidor.

## tecnologia
  dotnet core 3.1
  rabbitmq
  docker
  
para rodar o projeto utilize `docker-compose -f docker-composy.yml`
