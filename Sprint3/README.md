# Sprint3 - API de Gestão Escolar

Projeto desenvolvido para o desafio da Sprint 3 do curso Ford Enter

A aplicação consiste em uma API REST criada com ASP.NET Core, integrada a um banco de dados MySQL, utilizando Entity Framework Core, Migrations, autenticação JWT, autorização por perfis, Swagger para documentação e uma interface web simples para consumo da API.

---

## 📌 Objetivo do Projeto

O objetivo do projeto é simular um sistema de gestão escolar, permitindo o gerenciamento de:

- Alunos
- Professores
- Disciplinas
- Notas

A API permite realizar operações completas de CRUD, aplicar regras de negócio, proteger rotas com autenticação JWT e consumir os dados por meio de uma interface web integrada.

---

## 🛠️ Tecnologias Utilizadas

- C#
- ASP.NET Core Web API
- Entity Framework Core
- Pomelo
- MySQL
- JWT Bearer Authentication
- Swagger / Swashbuckle
- HTML
- CSS
- JavaScript
- Bootstrap


---

## 🧱 Arquitetura do Projeto

O projeto foi organizado em camadas para separar responsabilidades e facilitar a manutenção do código.

### Controllers

Responsáveis por receber as requisições HTTP, chamar os services e retornar as respostas adequadas para o cliente.

Exemplo:

- `AlunoController`
- `ProfessorController`
- `DisciplinaController`
- `NotaController`
- `AuthController`
- `DiretorController`
- `UsuarioController`
- `SolicitacaoAcessoController`

### Services

Responsáveis pela lógica de negócio da aplicação.

Exemplo:

- Cálculo da média do aluno
- Definição da situação do aluno
- Busca de professor por nome ao cadastrar uma disciplina
- Busca de aluno e disciplina ao cadastrar uma nota

### Repositories

Responsáveis pelo acesso ao banco de dados utilizando Entity Framework Core.

Essa camada centraliza as operações de consulta, cadastro, atualização e exclusão no banco.

### Models

Representam as entidades do banco de dados.

Entidades principais:

- `Aluno`
- `Professor`
- `Diretor`
- `Usuario`
- `SolicitacaoAcesso`
- `Disciplina`
- `Nota`

### DTOs

Utilizados para controlar os dados que entram e saem da API.

Foram criados DTOs de request e response para evitar expor diretamente as entidades do banco.

Exemplos:

- `AlunoRequest`
- `AlunoResponse`
- `ProfessorRequest`
- `ProfessorResponse`
- `DisciplinaRequest`
- `DisciplinaResponse`
- `NotaRequest`
- `NotaResponse`
- `LoginRequest`
- `TokenResponse`

---

## 🗃️ Banco de Dados

O banco de dados utilizado no projeto foi o **MySQL**.

A conexão é configurada no arquivo `appsettings.json`.

Exemplo de configuração:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=sprint3db;user=root;password=cimatec;"
  }
}  

```
Usuários de teste para autenticação JWT

Admin:
Email: admin@sprint3.com
Senha: 123456

Professor:
Email: professor@sprint3.com
Senha: 123456

Diretor:
Email: diretor@sprint3.com
Senha: 123456

Aluno:
Email: aluno@sprint3.com
Senha: 123456

Consulte também `../README_PORTAL_ESCOLAR.md` para a visão atualizada do portal, rotas, permissões, migration e comandos de teste.
