# Portal Escolar Sprint3

## 1. Descricao do Projeto

O Portal Escolar Sprint3 e um sistema de gerenciamento academico desenvolvido em ASP.NET Core com MySQL, autenticação JWT e interface web integrada.

O sistema centraliza o controle de alunos, professores, disciplinas, matriculas, notas e solicitacoes de acesso. Cada usuario acessa o portal conforme seu perfil:

- Aluno: visualiza suas disciplinas e boletim.
- Professor: visualiza alunos das suas disciplinas e lanca notas.
- Diretor: gerencia alunos, professores, disciplinas, matriculas, notas e solicitacoes de acesso.

O projeto foi desenvolvido para fins educacionais e de apresentacao, simulando um ambiente de gestao escolar.

## 2. Tecnologias Utilizadas

- Linguagem: C#
- Framework: ASP.NET Core Web API
- Banco de Dados: MySQL
- ORM: Entity Framework Core com Pomelo MySQL
- Segurança: JWT Bearer Authentication
- Documentacao de API: Swagger / OpenAPI
- Front-end: HTML, CSS, JavaScript e Bootstrap
- Containerizacao: Docker e Docker Compose

## 3. Instrucoes de Execucao

Para rodar o projeto localmente, siga os passos abaixo:

1. Clone o repositorio e acesse a pasta do projeto.

2. Configure o banco de dados MySQL no arquivo `Sprint3/appsettings.json` ou use o Docker Compose com o arquivo `.env`.

3. Restaure as dependencias e compile o projeto:

```bash
dotnet restore Sprint3/Sprint3.csproj
dotnet build Sprint3/Sprint3.csproj
```

4. Aplique as migrations do Entity Framework:

```bash
dotnet tool restore
dotnet dotnet-ef database update --project Sprint3/Sprint3.csproj --startup-project Sprint3/Sprint3.csproj
```

5. Execute a aplicacao:

```bash
dotnet run --project Sprint3/Sprint3.csproj
```

Acesse a URL exibida no terminal. Em ambiente local, normalmente:

```txt
http://localhost:5168
```

### Execucao com Docker

Tambem e possivel subir a API e o MySQL com Docker:

```bash
cp .env.example .env
docker compose up --build
```

A aplicacao ficara disponivel em:

```txt
http://localhost:8080
```

Para parar os containers:

```bash
docker compose down
```

Para apagar os dados antigos e recriar a base de demonstracao pelo seed:

```bash
docker compose down -v
docker compose up --build
```

## 4. Endpoints da API

Abaixo estao os principais endpoints disponiveis no sistema:

### Autenticacao

- `POST /api/Auth/login` - Realiza login e retorna o token JWT.
- `GET /api/Auth/me` - Retorna os dados do usuario autenticado.

### Solicitacoes de Acesso

- `POST /api/SolicitacaoAcesso` - Envia uma solicitacao publica de acesso.
- `GET /api/SolicitacaoAcesso` - Lista solicitacoes de acesso.
- `GET /api/SolicitacaoAcesso/{id}` - Busca uma solicitacao por ID.
- `POST /api/SolicitacaoAcesso/{id}/aprovar` - Aprova uma solicitacao.
- `POST /api/SolicitacaoAcesso/{id}/recusar` - Recusa uma solicitacao.

### Alunos

- `GET /api/Aluno` - Lista alunos.
- `GET /api/Aluno/{id}` - Busca aluno por ID.
- `POST /api/Aluno` - Cadastra aluno.
- `PUT /api/Aluno/{id}` - Atualiza aluno.
- `DELETE /api/Aluno/{id}` - Remove aluno.

### Professores

- `GET /api/Professor` - Lista professores.
- `GET /api/Professor/{id}` - Busca professor por ID.
- `POST /api/Professor` - Cadastra professor.
- `PUT /api/Professor/{id}` - Atualiza professor.
- `DELETE /api/Professor/{id}` - Remove professor.

### Disciplinas

- `GET /api/Disciplina` - Lista disciplinas.
- `GET /api/Disciplina/{id}` - Busca disciplina por ID.
- `POST /api/Disciplina` - Cadastra disciplina.
- `PUT /api/Disciplina/{id}` - Atualiza disciplina.
- `DELETE /api/Disciplina/{id}` - Remove disciplina.

### Matriculas

- `GET /api/Matricula` - Lista matriculas conforme o perfil do usuario.
- `GET /api/Matricula/{id}` - Busca matricula por ID.
- `POST /api/Matricula` - Matricula aluno em uma disciplina.
- `PUT /api/Matricula/{id}` - Atualiza matricula.
- `DELETE /api/Matricula/{id}` - Remove matricula.

### Notas

- `GET /api/Nota` - Lista notas conforme o perfil do usuario.
- `GET /api/Nota/{id}` - Busca nota por ID.
- `GET /api/Nota/aluno/{alunoId}` - Lista notas de um aluno.
- `POST /api/Nota` - Lanca nota para aluno matriculado.
- `PUT /api/Nota/{id}` - Atualiza nota.
- `DELETE /api/Nota/{id}` - Remove nota.

## 5. Usuarios de Teste

Os usuarios abaixo sao criados automaticamente no ambiente `Development`:

| Perfil | Email | Senha |
| --- | --- | --- |
| Diretor | `diretor@sprint3.com` | `123456` |
| Professor | `professor@sprint3.com` | `123456` |
| Aluno | `aluno@sprint3.com` | `123456` |

Tambem sao criados outros usuarios para demonstracao:

- Professores: `roberto.lima@sprint3.com`, `camila.fernandes@sprint3.com`
- Alunos: `lucas.almeida@sprint3.com`, `maria.oliveira@sprint3.com`, `ana.souza@sprint3.com`, `pedro.santos@sprint3.com`
- Senha padrao para todos: `123456`

## 6. Regras de Negocio

- O aluno visualiza somente suas proprias disciplinas e notas.
- O professor visualiza somente alunos matriculados nas disciplinas dele.
- O professor so pode lancar notas para alunos matriculados em suas disciplinas.
- O diretor e o perfil com maior permissao no sistema.
- Uma nota so pode ser lancada se o aluno estiver matriculado na disciplina.
- Solicitacoes de acesso ficam pendentes ate aprovacao ou recusa do diretor.
- Ao aprovar uma solicitacao, o sistema cria o usuario correspondente.
- No boletim, a situacao pode ser: Sem nota, Aprovado ou Reprovado.

## 7. Funcionalidades Principais

- Login com autenticação JWT.
- Controle de acesso por perfil.
- Solicitacao publica de acesso.
- Aprovacao e recusa de solicitacoes.
- Cadastro de alunos.
- Cadastro de professores.
- Cadastro de disciplinas.
- Matricula de alunos em disciplinas.
- Lancamento de notas.
- Visualizacao de boletim pelo aluno.
- Visualizacao de alunos por professor.
- Painel inicial personalizado por perfil.
- Interface web integrada com cards, listas e acoes rapidas.
