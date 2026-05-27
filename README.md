# Sprint3 - Portal Escolar

Sistema de gestão escolar desenvolvido em ASP.NET Core com MySQL, Entity Framework Core, autenticação JWT e interface web integrada.

O projeto permite gerenciar alunos, professores, diretores, disciplinas, matrículas, notas, usuários e solicitações de acesso. Cada usuário acessa o portal conforme seu perfil:

- Aluno: visualiza suas disciplinas e boletim.
- Professor: visualiza alunos das suas disciplinas e lança notas.
- Diretor: gerencia alunos, professores, disciplinas, matrículas, notas e solicitações de acesso.
- Admin: gerencia todos os módulos, incluindo usuários e diretores.

## Tecnologias

- C#
- ASP.NET Core Web API
- Entity Framework Core
- Pomelo EntityFrameworkCore MySQL
- MySQL
- JWT Bearer Authentication
- Swagger / OpenAPI
- HTML, CSS, JavaScript e Bootstrap
- xUnit

## Como Executar

1. Configure o MySQL no arquivo `Sprint3/appsettings.json`.

2. Restaure as dependências:

```bash
dotnet restore Sprint3/Sprint3.csproj
```

3. Restaure a ferramenta do Entity Framework:

```bash
dotnet tool restore
```

4. Aplique as migrations:

```bash
dotnet dotnet-ef database update --project Sprint3/Sprint3.csproj --startup-project Sprint3/Sprint3.csproj
```

5. Execute a aplicação:

```bash
dotnet run --project Sprint3/Sprint3.csproj
```

Acesse a URL exibida no terminal. Em ambiente local, normalmente:

```txt
http://localhost:5168
```

## Testes

```bash
dotnet test Sprint3.Tests/Sprint3.Tests.csproj
```

## Usuários de Teste

Os usuários abaixo são criados automaticamente no ambiente `Development`.

| Perfil | Email | Senha |
| --- | --- | --- |
| Diretor | `diretor@sprint3.com` | `123456` |
| Professor | `professor@sprint3.com` | `123456` |
| Aluno | `aluno@sprint3.com` | `123456` |

Também são criados outros usuários de demonstração:

- Professores: `roberto.lima@sprint3.com`, `camila.fernandes@sprint3.com`
- Alunos: `lucas.almeida@sprint3.com`, `maria.oliveira@sprint3.com`, `ana.souza@sprint3.com`, `pedro.santos@sprint3.com`
- Senha padrão para todos: `123456`

## Endpoints Principais

### Autenticação

- `POST /api/Auth/login`
- `GET /api/Auth/me`

### Alunos

- `GET /api/Aluno`
- `GET /api/Aluno/{id}`
- `POST /api/Aluno`
- `PUT /api/Aluno/{id}`
- `DELETE /api/Aluno/{id}`

### Professores

- `GET /api/Professor`
- `GET /api/Professor/{id}`
- `POST /api/Professor`
- `PUT /api/Professor/{id}`
- `DELETE /api/Professor/{id}`

### Diretores

- `GET /api/Diretor`
- `GET /api/Diretor/{id}`
- `POST /api/Diretor`
- `PUT /api/Diretor/{id}`
- `DELETE /api/Diretor/{id}`

### Disciplinas

- `GET /api/Disciplina`
- `GET /api/Disciplina/{id}`
- `POST /api/Disciplina`
- `PUT /api/Disciplina/{id}`
- `DELETE /api/Disciplina/{id}`

### Matrículas

- `GET /api/Matricula`
- `GET /api/Matricula/{id}`
- `POST /api/Matricula`
- `PUT /api/Matricula/{id}`
- `DELETE /api/Matricula/{id}`

### Notas

- `GET /api/Nota`
- `GET /api/Nota/{id}`
- `GET /api/Nota/aluno/{alunoId}`
- `POST /api/Nota`
- `PUT /api/Nota/{id}`
- `DELETE /api/Nota/{id}`

### Usuários

- `GET /api/Usuario`
- `GET /api/Usuario/{id}`
- `POST /api/Usuario`
- `PUT /api/Usuario/{id}`
- `DELETE /api/Usuario/{id}`

### Solicitações de Acesso

- `POST /api/SolicitacaoAcesso`
- `GET /api/SolicitacaoAcesso`
- `GET /api/SolicitacaoAcesso/{id}`
- `POST /api/SolicitacaoAcesso/{id}/aprovar`
- `POST /api/SolicitacaoAcesso/{id}/recusar`

## Regras de Negócio

- O aluno visualiza somente suas próprias disciplinas e notas.
- O professor visualiza somente alunos matriculados nas disciplinas dele.
- O professor só pode lançar notas para alunos matriculados em suas disciplinas.
- O diretor gerencia alunos, professores, disciplinas, matrículas, notas e solicitações de acesso.
- O admin possui acesso completo ao sistema.
- Uma nota só pode ser lançada se o aluno estiver matriculado na disciplina.
- Solicitações de acesso ficam pendentes até aprovação ou recusa.
- Ao aprovar uma solicitação, o sistema cria o usuário correspondente.
