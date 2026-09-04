# Biblioteca — stack Docker

O projeto inteiro é orquestrado pelo `docker-compose.yml` da raiz. O `Dockerfile`
multiestágio também fica na raiz e produz imagens separadas para a API e o frontend.

## Executar

```bash
cp .env.example .env
docker compose up --build
```

A cópia do `.env` é opcional: todos os valores possuem padrões adequados para
desenvolvimento.

| Serviço | Endereço externo | Função |
|---|---|---|
| Frontend | http://localhost:3000 | Interface e proxy reverso |
| API | http://localhost:8080 | ASP.NET Core |
| Swagger | http://localhost:3000/docs/ | Documentação da API pelo proxy |
| PostgreSQL | somente rede interna | Persistência |
| Redis | somente rede interna | Cache/healthcheck |

Credenciais de demonstração: `admin@biblioteca.local` / `admin123`.

O navegador chama `/api/*` no mesmo host do frontend. O Nginx encaminha essas
requisições para `backend:8080`, portanto não há URL de API fixa no JavaScript nem
problemas de CORS. PostgreSQL e Redis usam volumes nomeados e todos os serviços têm
healthcheck.

## Comandos úteis

```bash
docker compose ps
docker compose logs -f backend
docker compose down
```

Para remover também os dados persistidos, use explicitamente `docker compose down -v`.

Fora do Docker, a API continua usando SQLite por padrão. No Compose,
`Database__Provider=Postgres` seleciona PostgreSQL sem tentar reaproveitar migrations
específicas do SQLite.
