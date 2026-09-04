# BibliotecaAPI 📚

API REST para gestão de biblioteca desenvolvida em **ASP.NET Core 10** com **Entity Framework Core**, SQLite e PostgreSQL.

## Stack Tecnológica

| Tecnologia | Versão |
|---|---|
| .NET / ASP.NET Core | 10.0 |
| Entity Framework Core | 10.x |
| Banco de Dados | SQLite local / PostgreSQL no Docker |
| Cache e monitoramento | Redis |
| Documentação | Swagger / OpenAPI |

## Como Executar

### Pré-requisitos
- .NET 10 SDK

### ⚠️ Atenção — Configuração do PATH (Linux Mint)

Se ao rodar `dotnet` você receber o erro `[/usr/lib/dotnet/host/fxr] does not exist`, é porque o sistema possui uma instalação corrompida do dotnet em `/usr/bin/dotnet`. O SDK funcional está em `~/.dotnet`.

**Solução permanente** — abra um **novo terminal** e execute uma vez:

```bash
# Garante que ~/.dotnet tem prioridade no PATH
echo 'export DOTNET_ROOT="$HOME/.dotnet"' >> ~/.bashrc
echo 'export PATH="$HOME/.dotnet:$HOME/.dotnet/tools:$PATH"' >> ~/.bashrc
source ~/.bashrc
```

> Esta configuração já está aplicada se você usou o terminal do projeto antes. Basta **abrir um terminal novo** para que o `source ~/.bashrc` seja carregado automaticamente.

### Passos

```bash
# 1. Entrar na pasta do projeto
cd BibliotecaAPI

# 2. Restaurar dependências
dotnet restore

# 3. Executar a API (migrations são aplicadas automaticamente na inicialização)
dotnet run
```

A API estará disponível em: **http://localhost:5000** (porta exata aparece no terminal ao iniciar)

O Swagger (documentação interativa) estará em `http://localhost:{porta}/docs/`.

> O banco `biblioteca.db` é criado e migrado automaticamente na primeira execução.

---

## Estrutura do Projeto

```
BibliotecaAPI/
├── Configuration/     # Opções tipadas da aplicação
├── Controllers/       # Adaptadores HTTP, sem regras de negócio
├── Data/              # DbContext, inicialização, seed e Unit of Work
├── DTOs/              # Contratos separados por domínio
├── Extensions/        # Composição da aplicação e injeção de dependência
├── Exceptions/        # Exceções customizadas (NotFoundException, ConflictException)
├── Mappings/          # Conversão entre entidades e DTOs
├── Middleware/        # Tratamento global de erros (ProblemDetails)
├── Migrations/        # Histórico de migrações do banco
├── Models/            # Entidades de domínio
├── Repositories/      # Interfaces + implementações de acesso a dados
├── Services/          # Interfaces + implementações da lógica de negócio
├── Program.cs         # Composition root enxuto
└── appsettings.json   # String de conexão e configurações
```

---

## Endpoints

### Autores
| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/autores` | Lista todos os autores |
| `GET` | `/api/autores/{id}` | Busca autor por ID |
| `POST` | `/api/autores` | Cadastra novo autor |

**Payload POST:**
```json
{
  "nome": "Robert C. Martin",
  "dataNascimento": "1952-12-05",
  "nacionalidade": "Americana"
}
```

### Livros
| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/livros` | Lista livros (suporta `?titulo=` e `?autor=`) |
| `GET` | `/api/livros/{id}` | Busca livro por ID |
| `POST` | `/api/livros` | Cadastra novo livro |

**Payload POST:**
```json
{
  "isbn": "9780132350884",
  "titulo": "Clean Code",
  "anoPublicacao": 2008,
  "autorId": 1,
  "quantidade": 3
}
```

### Alunos
| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/alunos` | Lista todos os alunos |
| `GET` | `/api/alunos/{id}` | Busca aluno por ID |
| `POST` | `/api/alunos` | Cadastra novo aluno (matrícula única) |

### Empréstimos
| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/emprestimos` | Lista todos os empréstimos |
| `GET` | `/api/emprestimos/{id}` | Busca empréstimo por ID |
| `POST` | `/api/emprestimos` | Cria novo empréstimo |
| `PUT` | `/api/emprestimos/{id}/devolucao` | Registra devolução |

---

## Regras de Negócio

| Código HTTP | Situação |
|---|---|
| 201 | Cadastro realizado com sucesso |
| 400 | Dados inválidos (campos obrigatórios, e-mail inválido) |
| 404 | Recurso não encontrado |
| 409 | Livro sem estoque / Empréstimo duplicado / Devolução duplicada |

**Exemplo de resposta de erro (ProblemDetails):**
```json
{
  "title": "Conflito de negócio",
  "status": 409,
  "detail": "O livro não possui exemplares disponíveis."
}
```

---

## Casos de Teste (Checklist)

- [x] Cadastrar autor e livro → `201 Created`
- [x] Buscar ID inexistente → `404 Not Found`
- [x] Cadastrar aluno sem nome → `400 Bad Request`
- [x] Emprestar livro com quantidade 0 → `409 Conflict`
- [x] Devolver empréstimo já devolvido → `409 Conflict`
- [x] Cadastrar aluno com matrícula existente → `409 Conflict`

---

## Arquitetura e Padrões

- **Repository Pattern**: Separação entre acesso a dados e lógica de negócio.
- **Service Layer**: Toda regra de negócio fica nos Services, nunca nos Controllers.
- **Injeção de Dependência por interfaces**: Facilita testes e substituição de componentes.
- **Unit of Work**: cada caso de uso persiste todas as alterações em um único commit atômico.
- **Options Pattern**: configurações de banco e autenticação são tipadas e validadas na inicialização.
- **SOLID**: controllers, serviços, mapeadores, infraestrutura e bootstrap possuem responsabilidades isoladas.
- **Middleware Global de Erros**: Sem `try/catch` nos Controllers. Erros são capturados centralmente.
- **Programação Assíncrona**: `async/await` em todas as operações de banco.
- **DTOs**: Objetos específicos para entrada e saída, sem expor entidades do banco diretamente.
