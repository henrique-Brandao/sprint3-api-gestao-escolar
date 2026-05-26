# Portal Escolar Sprint3

## Execução

```bash
cd Sprint3
dotnet restore
cd ..
dotnet tool restore
dotnet dotnet-ef database update --project Sprint3/Sprint3.csproj --startup-project Sprint3/Sprint3.csproj
cd Sprint3
dotnet run
```

Abra a aplicação em `https://localhost:5001` ou na URL exibida pelo `dotnet run`.

## Docker

Para subir MySQL e API juntos:

```bash
cp .env.example .env
docker compose up --build
```

A API fica em `http://localhost:8080`.
No Docker Compose, `APPLY_MIGRATIONS=true` aplica as migrations na subida da API.

Para subir somente o banco e aplicar migrations localmente:

```bash
docker compose up -d db
dotnet tool restore
dotnet dotnet-ef database update --project Sprint3/Sprint3.csproj --startup-project Sprint3/Sprint3.csproj
dotnet run --project Sprint3/Sprint3.csproj
```

## Testes

```bash
dotnet test ../Sprint3.Tests/Sprint3.Tests.csproj
```

## Usuários de teste

- Admin: `admin@sprint3.com` / `123456`
- Diretor: `diretor@sprint3.com` / `123456`
- Professor: `professor@sprint3.com` / `123456`
- Aluno: `aluno@sprint3.com` / `123456`

Esses usuários são criados no ambiente `Development` pelo seed da aplicação.

## Rotas principais

- `POST /api/Auth/login`
- `GET /api/Auth/me`
- `GET|POST|PUT|DELETE /api/Aluno`
- `GET|POST|PUT|DELETE /api/Professor`
- `GET|POST|PUT|DELETE /api/Diretor`
- `GET|POST|PUT|DELETE /api/Disciplina`
- `GET|POST|PUT|DELETE /api/Nota`
- `GET|POST|PUT|DELETE /api/Usuario`
- `POST /api/SolicitacaoAcesso`
- `GET /api/SolicitacaoAcesso`
- `POST /api/SolicitacaoAcesso/{id}/aprovar`
- `POST /api/SolicitacaoAcesso/{id}/recusar`

## Permissões

- Aluno acessa somente o próprio portal, notas e disciplinas vinculadas.
- Professor gerencia notas e disciplinas, e consulta alunos.
- Diretor gerencia alunos, professores, disciplinas, notas e solicitações de acesso de Aluno/Professor.
- Admin gerencia todos os módulos, incluindo usuários, diretores e admins.

## Migration criada

- `20260525220327_PortalEscolarUsuariosSolicitacoes`

Ela adiciona `Diretores`, `Usuarios`, `SolicitacoesAcesso`, índices únicos de email e vínculos opcionais de usuário com aluno/professor/diretor.
