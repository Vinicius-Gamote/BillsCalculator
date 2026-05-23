# Gastos Control

Projeto para controle de gastos com backend C# Web API em arquitetura DDD, Entity Framework Core, SQL Server, autenticação JWT, frontend Angular responsivo e conteinerizacao Docker.

## Estrutura

- `backend/src/GastosControl.Domain`: entidades, enums e regras essenciais do dominio.
- `backend/src/GastosControl.Application`: DTOs, contratos, servicos de aplicacao e casos de uso.
- `backend/src/GastosControl.Infrastructure`: EF Core, DbContext, repositorios, hash de senha e geracao de JWT.
- `backend/src/GastosControl.Api`: controllers REST, autenticacao, Swagger e inicializacao do banco.
- `frontend`: Angular com login, dashboard, filtros, categorias e lancamentos.

## Rodando com Docker

```bash
docker compose up --build
```

Depois acesse:

- Frontend: http://localhost:4200
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger

Usuario demo criado em ambiente de desenvolvimento:

- Email: `demo@gastos.local`
- Senha: `Demo@123`

## Rodando localmente

Backend:

```bash
dotnet restore
dotnet run --project backend/src/GastosControl.Api/GastosControl.Api.csproj --urls http://localhost:5000
```

Frontend:

```bash
cd frontend
npm install
npm start
```

## Principais recursos

- Cadastro e login simples com JWT.
- Categorias por tipo de lancamento.
- Entradas e saidas mensais e anuais separadas.
- Filtros por categoria, tipo e periodo.
- Dashboard com comparativo mensal, totais por categoria e ultimos lancamentos.
- Persistencia em SQL Server via Entity Framework Core.
