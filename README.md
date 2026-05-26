# Sprint3 - Sistema de Gestão Escolar

Projeto de conclusão de curso profissionalizante com uma API REST em ASP.NET Core, banco MySQL, autenticação JWT, autorização por perfis e uma interface web integrada para gestão escolar.

## Funcionalidades

- Login com autenticação JWT.
- Perfis de acesso: Admin, Diretor, Professor e Aluno.
- Cadastro e consulta de alunos, professores, diretores, disciplinas, notas e usuários.
- Solicitação pública de acesso para aluno/professor.
- Aprovação ou recusa de solicitações por Admin/Diretor.
- Portal do aluno para consulta de notas.
- Controle de permissões por perfil.
- Migrations automáticas no Docker Compose.

## Tecnologias

- C#
- ASP.NET Core Web API
- Entity Framework Core
- MySQL
- Pomelo Entity Framework Core MySQL
- JWT Bearer Authentication
- Swagger
- HTML, CSS e JavaScript
- Bootstrap
- Docker e Docker Compose

## Estrutura do projeto

```txt
.
├── Sprint3/                 # API, frontend estático e regras da aplicação
├── Sprint3.Tests/           # Testes automatizados
├── Dockerfile               # Imagem da API
├── docker-compose.yml       # API + MySQL
├── .env.example             # Exemplo de variáveis de ambiente
└── README.md
```

## Usuários de teste

Os usuários abaixo são criados automaticamente no ambiente `Development`.

| Perfil | Email | Senha |
| --- | --- | --- |
| Admin | `admin@sprint3.com` | `123456` |
| Diretor | `diretor@sprint3.com` | `123456` |
| Professor | `professor@sprint3.com` | `123456` |
| Aluno | `aluno@sprint3.com` | `123456` |

## Como rodar com Docker

Requisitos:

- Docker
- Docker Compose

Crie o arquivo de ambiente:

```bash
cp .env.example .env
```

Suba a aplicação:

```bash
docker compose up --build
```

Acesse:

```txt
http://localhost:8080
```

Para parar:

```bash
docker compose down
```

Para apagar também os dados do banco e começar do zero:

```bash
docker compose down -v
```

## Como rodar localmente sem Docker

Requisitos:

- .NET SDK 10
- MySQL rodando localmente
- Banco configurado em `Sprint3/appsettings.json`

Restaure e compile:

```bash
dotnet restore Sprint3/Sprint3.csproj
dotnet build Sprint3/Sprint3.csproj
```

Aplique as migrations:

```bash
dotnet tool restore
dotnet dotnet-ef database update --project Sprint3/Sprint3.csproj --startup-project Sprint3/Sprint3.csproj
```

Rode a aplicação:

```bash
dotnet run --project Sprint3/Sprint3.csproj
```

Acesse a URL exibida no terminal. No perfil local padrão:

```txt
http://localhost:5168
```

## Testes

```bash
dotnet test Sprint3.Tests/Sprint3.Tests.csproj
```

## Variáveis de ambiente

O Docker Compose lê as variáveis do arquivo `.env`.

| Variável | Descrição |
| --- | --- |
| `MYSQL_ROOT_PASSWORD` | Senha do usuário root do MySQL |
| `MYSQL_DATABASE` | Nome do banco |
| `MYSQL_PORT` | Porta local do MySQL |
| `API_PORT` | Porta local da aplicação |
| `ASPNETCORE_ENVIRONMENT` | Ambiente da aplicação |
| `APPLY_MIGRATIONS` | Aplica migrations ao iniciar a API |
| `JWT_SECRET_KEY` | Chave usada para assinar tokens JWT |
| `JWT_ISSUER` | Emissor do token |
| `JWT_AUDIENCE` | Público do token |
| `JWT_EXPIRATION_MINUTES` | Tempo de expiração do token |

Para um deploy público, troque pelo menos:

- `MYSQL_ROOT_PASSWORD`
- `JWT_SECRET_KEY`
- `ASPNETCORE_ENVIRONMENT`

## Deploy em VPS

Para este projeto, o caminho mais simples é usar uma VPS com Ubuntu e Docker. A aplicação já está preparada para subir com `docker compose`, então você não precisa instalar .NET nem MySQL manualmente no servidor.

Resumo do processo:

1. Criar uma VPS Ubuntu.
2. Acessar por SSH.
3. Instalar Docker e Docker Compose.
4. Enviar ou clonar o projeto no servidor.
5. Criar o arquivo `.env`.
6. Rodar `docker compose up -d --build`.
7. Acessar pelo IP público da VPS na porta configurada.

Exemplo de comandos no servidor:

```bash
sudo apt update
sudo apt upgrade -y
```

Instale Docker seguindo a documentação oficial:

```txt
https://docs.docker.com/engine/install/ubuntu/
```

Depois, dentro da pasta do projeto:

```bash
cp .env.example .env
nano .env
docker compose up -d --build
```

Se `API_PORT=8080`, o acesso fica assim:

```txt
http://IP_DA_VPS:8080
```

Para ver logs:

```bash
docker compose logs -f api
```

Para atualizar depois de enviar novas alterações:

```bash
docker compose down
docker compose up -d --build
```

## Rotas principais da API

- `POST /api/Auth/login`
- `GET /api/Auth/me`
- `GET|POST|PUT|DELETE /api/Aluno`
- `GET|POST|PUT|DELETE /api/Professor`
- `GET|POST|PUT|DELETE /api/Diretor`
- `GET|POST|PUT|DELETE /api/Disciplina`
- `GET|POST|PUT|DELETE /api/Nota`
- `GET|POST|DELETE /api/Usuario`
- `POST /api/SolicitacaoAcesso`
- `GET /api/SolicitacaoAcesso`
- `POST /api/SolicitacaoAcesso/{id}/aprovar`
- `POST /api/SolicitacaoAcesso/{id}/recusar`

## Permissões

| Perfil | Permissões principais |
| --- | --- |
| Aluno | Visualiza o próprio portal, disciplinas e notas vinculadas |
| Professor | Gerencia notas e disciplinas, consulta alunos |
| Diretor | Gerencia alunos, professores, disciplinas, notas e solicitações |
| Admin | Gerencia todos os módulos, usuários, diretores e permissões |

## Roteiro sugerido para apresentação

1. Mostrar a tela de login e os perfis disponíveis.
2. Entrar como Admin e apresentar o painel administrativo.
3. Criar ou consultar alunos, professores e disciplinas.
4. Demonstrar uma solicitação de acesso pela tela pública.
5. Aprovar a solicitação como Admin ou Diretor.
6. Entrar com o usuário aprovado.
7. Lançar uma nota como Professor.
8. Entrar como Aluno e visualizar o boletim.
9. Explicar rapidamente a autenticação JWT, as permissões por perfil e o uso de Docker.

## Observações

Este projeto foi desenvolvido para fins educacionais. Para uso em produção real, recomenda-se configurar HTTPS, domínio, backup do banco, segredos fortes, política de senha mais completa e monitoramento da aplicação.
