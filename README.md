# Tech Challenge FIAP - Frames Video

## 📖 Visão Geral

Este projeto é uma aplicação .NET Core 8.0 que faz parte do Tech Challenge da FIAP. A aplicação tem como objetivo processar vídeos enviados pelos usuários, extraindo frames (quadros) dos vídeos e disponibilizando-os como arquivos compactados (.zip) através do Amazon S3.

### Funcionalidades Principais

- **Upload de Vídeos**: Recebe vídeos em diversos formatos (MP4, AVI, MOV, MKV, FLV, WMV, WebM)
- **Extração de Frames**: Utiliza FFMpeg para extrair frames dos vídeos (1 frame por segundo)
- **Compactação**: Cria arquivos ZIP contendo todos os frames extraídos
- **Armazenamento em Nuvem**: Upload automático dos arquivos ZIP para Amazon S3
- **Monitoramento de Status**: Acompanhamento em tempo real do status de processamento
- **Download de Arquivos**: API para download dos arquivos processados

## 🏗️ Arquitetura

O projeto segue os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**, organizando o código em camadas bem definidas:

### Estrutura de Pastas

```
src/
├── Core/                           # Camada de domínio e aplicação
│   ├── fiap.Application/          # Casos de uso e lógica de aplicação
│   │   ├── VideoUploadApplication.cs
│   │   └── Interfaces/
│   └── fiap.Domain/               # Entidades e regras de negócio
│       ├── Entities/
│       │   ├── VideoUpload.cs
│       │   ├── Usuario.cs
│       │   └── StatusUpload.cs
│       └── Interfaces/
├── Infrastructure/                 # Camada de infraestrutura
│   ├── Data/
│   │   └── fiap.Repositories/     # Repositórios e acesso a dados
│   └── Services/
│       └── fiap.Services/         # Serviços externos (S3, SQS)
├── Presentation/                   # Camada de apresentação
│   └── fiap.API/                  # API REST
│       ├── Controllers/
│       │   └── VideoUploadConvertController.cs
│       └── Program.cs
└── Tests/                         # Testes unitários
    └── fiap.Tests/
```

### Principais Entidades

#### VideoUpload
```csharp
public class VideoUpload
{
    public string Id { get; set; }                    // GUID único
    public string NomeArquivoOrigem { get; set; }     // Nome do arquivo original
    public string NomeArquivoGerado { get; set; }     // Nome do ZIP gerado
    public string UrlS3 { get; set; }                 // URL do arquivo no S3
    public Usuario Usuario { get; set; }              // Dados do usuário
    public StatusUpload StatusUpload { get; set; }    // Status atual do processamento
    public DateTime DataUpload { get; set; }          // Data/hora do upload
}
```

#### StatusUpload (Enum)
```csharp
public enum StatusUpload
{
    Enviado = 1,           // Arquivo enviado para a API
    Processando = 2,       // Extraindo frames do vídeo
    Finalizado = 3,        // Processamento concluído com sucesso
    Cancelado = 4,         // Processamento cancelado
    ErroProcessamento = 5, // Erro durante o processamento
    SalvandoS3 = 6,       // Fazendo upload para o S3
    ArquivoSalvoS3 = 7    // Arquivo salvo no S3 com sucesso
}
```

## 🚀 Tecnologias Utilizadas

### Backend
- **.NET Core 8.0**: Framework principal
- **ASP.NET Core Web API**: API REST
- **FFMpegCore**: Processamento de vídeo e extração de frames
- **Serilog**: Sistema de logging estruturado
- **OpenTelemetry**: Observabilidade e telemetria

### Cloud & Storage
- **Amazon S3**: Armazenamento de arquivos
- **Amazon SQS**: Fila de mensagens para status
- **AWS SDK**: Integração com serviços AWS

### Observabilidade
- **Health Checks**: Monitoramento de saúde da aplicação
- **Swagger/OpenAPI**: Documentação da API
- **OpenTelemetry**: Traces distribuídos

### DevOps
- **Docker**: Containerização
- **Kubernetes**: Orquestração de containers
- **Horizontal Pod Autoscaler (HPA)**: Auto scaling
- **Service Account**: Autenticação AWS via IAM Roles

## 📋 Pré-requisitos

### Para Desenvolvimento Local
- .NET 8.0 SDK
- Docker Desktop
- Conta AWS configurada
- FFMpeg instalado

### Para Deploy
- Cluster Kubernetes
- AWS CLI configurado
- kubectl configurado
- Permissões adequadas no AWS IAM

## 🔧 Configuração Local

### 1. Clone o Repositório
```bash
git clone https://github.com/lmunarim/tech-challenge-fiap-frames-video.git
cd tech-challenge-fiap-frames-video
```

### 2. Configuração do AWS
Configure suas credenciais AWS no arquivo `appsettings.json`:

```json
{
  "AWS": {
    "Region": "us-east-1",
    "Profile": "default"
  }
}
```

### 3. Build da Aplicação
```bash
dotnet restore
dotnet build
```

### 4. Executar com Docker
```bash
docker build -t frames-video .
docker run -p 8080:8080 frames-video
```

## 📊 API Endpoints

### POST /api/VideoUploadConvert/Upload
Faz upload de um vídeo para processamento.

**Parâmetros:**
- `video` (form-data): Arquivo de vídeo
- `nome` (string): Nome do usuário
- `email` (string): Email do usuário

**Formatos Suportados:**
- .mp4, .avi, .mov, .mkv, .flv, .wmv, .webm

**Resposta:**
```json
{
  "success": true,
  "zip": "guid.zip",
  "url": "https://bucket.s3.amazonaws.com/guid.zip"
}
```

### GET /api/VideoUploadConvert/BaixarZip
Baixa um arquivo ZIP processado.

**Parâmetros:**
- `fileName` (query): Nome do arquivo ZIP

### GET /api/VideoUploadConvert/Status
Verifica o status dos arquivos processados.

**Resposta:**
```json
{
  "total": 2,
  "files": [
    {
      "filename": "video.zip",
      "size": 1024000,
      "created_at": "2025-01-15 10:30:00",
      "download_url": "/download/video.zip"
    }
  ]
}
```

### GET /api/health
Health check da aplicação.

### GET /api/metrics
Métricas da aplicação.

## 🎯 Fluxo de Processamento

1. **Upload**: Usuário envia vídeo via API
2. **Validação**: Sistema valida formato do arquivo
3. **Processamento Assíncrono**: 
   - Salva vídeo temporariamente
   - Extrai frames usando FFMpeg (1 frame/segundo)
   - Cria arquivo ZIP com todos os frames
4. **Upload S3**: Envia ZIP para Amazon S3
5. **Notificação**: Atualiza status via SQS
6. **Limpeza**: Remove arquivos temporários

## ☸️ Deploy no Kubernetes

### Estrutura dos Manifests

#### 1. Service Account (`app-service-account.yaml`)
```yaml
apiVersion: v1
kind: ServiceAccount
metadata:
  annotations:
    eks.amazonaws.com/role-arn: arn:aws:iam::147997141255:role/role-pod-access-fiap-api
  name: frames-video-sa
```

**Funcionalidade:**
- Configura autenticação com AWS via IAM Roles for Service Accounts (IRSA)
- Permite que os pods acessem serviços AWS (S3, SQS) sem credenciais hardcoded
- Inclui configuração do Metrics Server para HPA

#### 2. Deployment (`app-deployment.yaml`)
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: fiap-fase-5
  labels: 
     app: fiap-fase-5
spec:
  selector:
    matchLabels:
      app: fiap-fase-5
      version: v01
  template:
    metadata:
      labels:
        app: fiap-fase-5
        version: v01
    spec:
      serviceAccountName: frames-video-sa
      containers:
      - name: fiap-fase-5
        image: DOCKER_IMAGE
        resources:
          requests:
            memory: "100Mi"
            cpu: "100m"
          limits:
            memory: "200Mi"
            cpu: "200m"
        ports:
        - containerPort: 80
```

**Características:**
- **Resource Management**: Definição de requests e limits para CPU e memória
- **Security**: Usa Service Account específico para acesso AWS
- **Image Pull**: Configurado para usar Docker registry privado
- **Environment**: Configurado para ambiente de desenvolvimento

#### 3. Service (`app-service.yaml`)
```yaml
apiVersion: v1
kind: Service
metadata:
  name: fiap-fase-5-service
  labels:
    app: fiap-fase-5
spec:
  selector:
    app: fiap-fase-5 
  ports:
  - name: http
    port: 80
    targetPort: 8080
  - name: https
    port: 443
    targetPort: 8080
  type: LoadBalancer
```

**Funcionalidade:**
- **Load Balancer**: Expõe a aplicação externamente
- **Port Mapping**: Mapeia portas 80/443 para 8080 (porta da aplicação)
- **Service Discovery**: Permite acesso interno entre pods

#### 4. Horizontal Pod Autoscaler (`app-hpa.yaml`)
```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: fiap-fase-3-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: fiap-fase-3
  minReplicas: 1
  maxReplicas: 10
  metrics:
    - type: Resource
      resource:
        name: cpu
        target:
          type: Utilization
          averageUtilization: 50
```

**Características:**
- **Auto Scaling**: Escala automaticamente de 1 a 10 replicas
- **CPU-based**: Escala quando uso de CPU > 50%
- **Cost Optimization**: Reduz custos mantendo mínimo de 1 replica

### Deploy Steps

#### 1. Preparar Imagem Docker
```bash
# Build da imagem
docker build -t your-registry/frames-video:latest .

# Push para registry
docker push your-registry/frames-video:latest
```

#### 2. Configurar Manifests
```bash
# Substituir DOCKER_IMAGE pela imagem real
sed -i 's/DOCKER_IMAGE/your-registry\/frames-video:latest/g' k8s/app-deployment.yaml
```

#### 3. Aplicar Manifests
```bash
# Aplicar Service Account primeiro (dependência)
kubectl apply -f k8s/app-service-account.yaml

# Aplicar Deployment
kubectl apply -f k8s/app-deployment.yaml

# Aplicar Service
kubectl apply -f k8s/app-service.yaml

# Aplicar HPA
kubectl apply -f k8s/app-hpa.yaml
```

#### 4. Verificar Deploy
```bash
# Verificar pods
kubectl get pods -l app=fiap-fase-5

# Verificar serviços
kubectl get services

# Verificar HPA
kubectl get hpa

# Verificar logs
kubectl logs -l app=fiap-fase-5 -f
```

## 📈 Monitoramento e Observabilidade

### Health Checks
A aplicação expõe endpoints de health check:
- `/api/health`: Status geral da aplicação
- `/api/metrics`: Métricas detalhadas

### Logging
- **Serilog**: Logging estruturado
- **Request Logging**: Log de todas as requisições HTTP
- **Error Tracking**: Captura e log de exceções

### Telemetria
- **OpenTelemetry**: Traces distribuídos
- **HTTP Instrumentation**: Rastreamento de requisições
- **ASP.NET Core Instrumentation**: Métricas de performance

### Métricas do Kubernetes
- **CPU/Memory Usage**: Monitoramento de recursos
- **Pod Status**: Estado dos pods
- **HPA Metrics**: Métricas de auto scaling

## 🔒 Segurança

### AWS Security
- **IAM Roles**: Acesso seguro aos serviços AWS
- **Service Account**: Autenticação via IRSA
- **S3 Bucket Policies**: Controle de acesso aos arquivos

### Kubernetes Security
- **Service Accounts**: Isolamento de permissões
- **Resource Limits**: Prevenção de resource exhaustion
- **Network Policies**: Controle de tráfego de rede

### Application Security
- **File Validation**: Validação de tipos de arquivo
- **Error Handling**: Tratamento seguro de exceções
- **CORS**: Configuração adequada de CORS

## 🚀 Escalabilidade

### Horizontal Scaling
- **HPA**: Auto scaling baseado em CPU
- **Load Balancer**: Distribuição de carga
- **Stateless Design**: Aplicação sem estado para fácil scaling

### Performance
- **Async Processing**: Processamento assíncrono de vídeos
- **Resource Management**: Otimização de CPU e memória
- **Cleanup**: Limpeza automática de arquivos temporários

## 🛠️ Troubleshooting

### Problemas Comuns

#### 1. Erro de Upload para S3
```bash
# Verificar permissões AWS
kubectl describe serviceaccount frames-video-sa

# Verificar logs
kubectl logs -l app=fiap-fase-5 | grep S3
```

#### 2. FFMpeg não encontrado
```bash
# Verificar se FFMpeg está instalado no container
kubectl exec -it <pod-name> -- which ffmpeg
```

#### 3. HPA não funcionando
```bash
# Verificar Metrics Server
kubectl get pods -n kube-system | grep metrics-server

# Verificar métricas
kubectl top nodes
kubectl top pods
```

#### 4. Service não acessível
```bash
# Verificar LoadBalancer
kubectl get svc fiap-fase-5-service

# Verificar endpoints
kubectl get endpoints fiap-fase-5-service
```

## 📝 Desenvolvimento

### Adicionando Nova Funcionalidade

1. **Domain**: Criar/modificar entidades em `fiap.Domain`
2. **Application**: Implementar casos de uso em `fiap.Application`
3. **Infrastructure**: Adicionar serviços em `fiap.Services`
4. **API**: Criar controllers em `fiap.API`
5. **Tests**: Adicionar testes em `fiap.Tests`

### Boas Práticas

- Seguir princípios SOLID
- Implementar testes unitários
- Usar injeção de dependência
- Implementar logging estruturado
- Seguir padrões de REST API

## 📞 Suporte

Para questões relacionadas ao projeto:

1. **Issues**: Usar GitHub Issues para bugs e melhorias
2. **Documentation**: Consultar esta documentação
3. **Logs**: Verificar logs da aplicação para troubleshooting


## Recursos Adicionais

- [Desenho Arquitetura](docs/Desenho%20Arquitetura.jpg)
- [Demonstração em vídeo](https://youtu.be/Wy4g0rllg80)

**Desenvolvido para o Tech Challenge FIAP - 9SOAT**