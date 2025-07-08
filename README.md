# 9SOAT - Tech Challenge FIAP - Fase 3

## Visão Geral do Projeto

Este projeto consiste no desenvolvimento de uma solução que integra os conhecimentos obtidos em todas as disciplinas da fase. O objetivo é desenvolver um sistema de autoatendimento para uma lanchonete que está expandindo, visando melhorar a eficiência no atendimento e gerenciamento de pedidos.
O projeto contempla a implementação de APIs para cadastro de clientes, produtos, pedidos e pagamentos e foi desenvolvido utilizando as boas práticas do 'Clean Architecture' com a linguagem C# com .NET8 e banco de dados RDS.

## Problema

A lanchonete enfrenta dificuldades no atendimento devido à falta de um sistema de controle de pedidos, resultando em confusão, atrasos e insatisfação dos clientes. Para resolver isso, será implementado um sistema de autoatendimento com as seguintes funcionalidades:

 - Pedido: Interface de seleção para clientes, com opções de identificação via CPF, cadastro ou anônimo. Os clientes podem montar combos de lanche, acompanhamento e bebida.
 - Pagamento: Integração com QRCode do Mercado Pago.
 - Acompanhamento: Monitoramento do progresso do pedido (Recebido, Em preparação, Pronto, Finalizado).
 - Entrega: Notificação ao cliente quando o pedido estiver pronto para retirada.
 - Acesso Administrativo: Gerenciamento de clientes, produtos, categorias e acompanhamento de pedidos.

 ## Arquitetura

 A arquitetura do projeto segue o padrão de camadas, separando responsabilidades em diferentes projetos:
   - Presentation: Contém os controladores da API.
   - Application: Contém as interfaces e implementações das regras de negócio.
   - Domain: Contém as entidades e interfaces de repositório.
   - Infrastructure: Contém a implementação dos repositórios e a configuração do banco de dados.

 ### Requisitos do negócio

 - Gerenciamento de Pedidos: Interface de autoatendimento para clientes.
 - Integração de Pagamento: QRCode do Mercado Pago.
 - Monitoramento de Pedidos: Acompanhamento do status do pedido.
 - Notificação de Entrega: Notificação ao cliente quando o pedido estiver pronto.
 - Painel Administrativo: Gerenciamento de clientes, produtos e pedidos.

 ### Requisitos de Infraestrutura

 - Kubernetes: Utilização do AWS EKS (Elastic Kubernetes Service) para escalabilidade e confiabilidade.
 - Horizontal Pod Autoscaler (HPA): Escalonamento automático de pods com base no uso de CPU e memória.
 - Serviços AWS: Utilização de RDS para gerenciamento de banco de dados e CloudWatch para monitoramento e logging.
 - Utilização de arquivos de configuração para "Service", "Service Account" e "Deployment" (arquivos manifestos do tipo yaml).
 - Utilização de deployment service através do Github actions para expor a aplicação.
 - Utilização do AWS Secrets para armazenamento seguro de valores sensíveis para acesso ao banco de dados e ao serviço externo de meio de pagamentos (MercadoPado).
 - Integração com os serviços do MercadoPago para criação de Ordem de Pagamento e confirmação de processamento de Pagamentos.

 ### Desenho de arquitetura da solução proposta

 ![Desenho Arquitetura](docs/Desenho%20Arquitetura.jpg)

## Como Iniciar o Projeto Localmente

### Pré-requisitos

- Clonar o repositório no link: https://github.com/gisele-cesar/tech-challenge-fiap/tree/master
- Iniciar instância banco de dados AWS RDS 'db-rds-fiap'

### Passos para executar o projeto

1. Clone o repositório e acesse a pasta do projeto:

```bash
   git clone https://github.com/gisele-cesar/tech-challenge-fiap.git
   cd tech-challenge-fiap
```

2. Executar o Projeto:
 -  Abra o projeto no Visual Studio.
 -  Selecione o projeto fiap.API como projeto de inicialização.
 -  Pressione F5 para iniciar a aplicação.

4.	Acessar a API:
   
   Abra o navegador e acesse https://localhost:44322/swagger/index.html para acessar a interface do Swagger e testar as APIs.

## Implantação da nuvem (AWS EKS)

### Pré-requisitos:

 - Conta AWS
 - Criação de um container ECR (Amazon Elastic Container Registry) para publicação da imagem
 - AWS CLI configurado
 - kubectl configurado
 - EKS Cluster configurado

 ***Obs.: É possível obter as configurações acima através do link: https://dlmade.medium.com/ci-cd-with-github-action-and-aws-eks-5fd9714010cd


1. Criação e configuração do EKS Cluster: 

 Nesta fase de entrega do Tech Challenge, estamos implementando as melhores práticas de CI/CD para as aplicações, segregando os códigos em repositórios, portanto a criação de infra para o EKS está sendo realizada por meio de Infra como código utilizando Terraform no repo [tech-challenge-fiap-infra-kubectl](https://github.com/gisele-cesar/tech-challenge-fiap-infra-kubectl).


2. Deploy da aplicação

 - Acessar o Github e realizar o Pull Request para a branch [develop] para criação da action e deploy da aplicação na AWS
 - Na aba "Pull Requests" do Github, confirme o merge pull request
 - Em seguida, vá até a aba "Actions do Gihub e acompanhe o pipeline gerado
 - Se executar com sucesso (sem erros), o deploy para o ECR foi realizado com sucesso.
 - Para obter o link de acesso, acesse a conta AWS, consulte o "Load Balancers" no caminho EC2 -> Load Balancers
 - Clique para acessar e visualizar o nome e seus detalhes
 - Copie o nome do DNS gerado e cole no browser adicionando no final 'api/health'
 - Copie o nome do DNS gerado e cole no browser adicionando no final a api a ser consultada ou substitua o parâmetro {{baseUrl}} da collection do Postman

4. Acessar a API: Use o IP externo do serviço LoadBalancer para acessar a API.


## Como testar as API's do Projeto

### Pré-requisitos

- Instância banco de dados AWS RDS 'db-rds-fiap' disponível
- Acessar o swagger em https://localhost:44322/swagger/index.html (para rodar localmente)
- Utilizar a colletion do Postman substituindo o parâmetro {{baseUrl}} pelo DNS gerado na instância do LoadBalancer  (para rodar na AWS)

### Ordem de execução das APIs

1. Acessar a API Cliente (POST /Cliente) para realizar cadastro de um ou mais clientes.
2. Acessar a API Produto (POST /Produto) para realizar cadastro de um ou mais produtos (na documentação das API's /Produto consta lista de Id's de categoria do produto para o cadastro).
3. Acessar a API de Pedido (POST /Pedido) para realizar cadastro de um mais pedidos.

- Obs.: Exemplo de requisição consta na documentação do swagger e também na collection do postman para cada uma das API's acima.
- Obs.2: é necessário que seja realizado o cadastro de cliente e de produto antes de realizar o cadastro de pedidos.

## Demonstração em Vídeo

[Acesse o link do YouTube](https://www.youtube.com/watch?v=A6mOvoZ_910)

## Collection do Postman

Acesse clicando: [collection](docs/9SOAT%20-%20Tech%20Challenge%20-%20Fase%202.postman_collection.json)

## Estrutura do Repositório

 - src/: Código-fonte do projeto

 - k8s/: Arquivos de configuração "Service", "Service Account" e "Deployment"

 - Dockerfile: Configuração do Docker para a publicação da imagem da aplicação na AWS

 - deploy.yml: Arquivo de configuração para o deploy via Github actions
