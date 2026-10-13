# Partners API

API RESTful em .NET 10 para gestão de parceiros de negócio (clientes, fornecedores), construída com **Clean Architecture**, **AutoMapper**, **EF Core** e **Injeção de Dependências**.

---

## Arquitetura

```
Partners.Api          -> Controllers, Middlewares (presentation)
Partners.Aplication   -> Services, DTOs, Mappings, Exceptions (regras de negócio)
Partners.Domain       -> Entidades, Enums, Interfaces de Repositório
Partners.Infra.Data   -> DbContext, Repositórios, Migrations (persistência)
Partners.Infra.Ioc    -> Composition Root (DI)
```

**Regra de dependências:** Domínio não conhece ninguém; Aplicação conhece Domínio; Infraestrutura conhece Domínio; API conhece Aplicação. A Composition Root (Ioc) conhece tudo.

---

## Tecnologias

- .NET 10 (C#)
- EF Core 10 + SQL Server
- AutoMapper 16 (mapeamento DTO ↔ Entidade)
- Swagger (documentação interativa)
- xUnit + Moq + FluentAssertions (testes)

---

## Bibliotecas (NuGet)

### Código de produção

| Pacote | Versão | Projeto | Função |
|-------|--------|---------|--------|
| `Microsoft.AspNetCore.OpenApi` | 10.0.12 | Api | Documentação OpenAPI |
| `Swashbuckle.AspNetCore` | 10.3.0 | Api | UI do Swagger |
| `Microsoft.EntityFrameworkCore.Design` | 10.0.12 | Api | Design-time do EF (migrations) |
| `AutoMapper` | 16.2.0 | Aplication | Mapeamento automático DTO ↔ Entidade |
| `Microsoft.EntityFrameworkCore.SqlServer` | 10.0.12 | Infra.Data | Provider SQL Server do EF Core |
| `Microsoft.EntityFrameworkCore.Tools` | 10.0.12 | Infra.Data | CLI do EF (dotnet ef) |

### Testes

| Pacote | Versão | Projetos | Função |
|-------|--------|---------|--------|
| `xunit` | 2.9.0 | todos os testes | Framework de testes |
| `xunit.runner.visualstudio` | 2.8.2 | todos os testes | Runner do Visual Studio |
| `Microsoft.NET.Test.Sdk` | 17.11.0 | todos os testes | SDK de testes |
| `Moq` | 4.20.70 | Aplication.Tests, Api.Tests | Mocking de dependências |
| `FluentAssertions` | 6.12.0 | todos os testes | Asserções fluentes |
| `Microsoft.AspNetCore.Mvc.Testing` | 10.0.12 | E2E.Tests | WebApplicationFactory |
| `Microsoft.EntityFrameworkCore.InMemory` | 10.0.12 | E2E.Tests | BD em memória para testes |

### Referências entre projetos

```
Partners.Api          -> Partners.Aplication, Partners.Infra.Ioc
Partners.Aplication   -> Partners.Domain
Partners.Infra.Data   -> Partners.Domain
Partners.Infra.Ioc    -> Partners.Aplication, Partners.Domain, Partners.Infra.Data
```

> O `Partners.Domain` não referencia nenhum projeto nem pacote NuGet — é a camada mais isolada (princípio da Arquitetura Limpa).

---

## Endpoints

| Método | Rota | Status | Descrição |
|--------|------|--------|-----------|
| GET | `/api/partners` | 200 | Lista paginada com filtros |
| GET | `/api/partners/{id}` | 200 / 404 | Obter por ID |
| POST | `/api/partners` | 201 / 400 / 409 | Criar |
| PUT | `/api/partners/{id}` | 200 / 404 / 409 | Atualizar |

### GET /api/partners — Query parameters

| Parâmetro | Tipo | Default | Descrição |
|-----------|------|---------|----------|
| `search` | string? | null | Pesquisa por Name (LIKE) ou TaxNumber (se numérico) |
| `type` | PartnerType? | null | CUSTOMER, SUPPLIER ou BOTH |
| `includeInactive` | bool | false | Incluir inativos |
| `page` | int | 1 | Página (min 1) |
| `pageSize` | int | 20 | Itens por página (1-100) |

**Resposta:** `PaginatedResult<PartnerDetailsDto>`
```json
{
  "items": [ { "id": "...", "name": "Acme", ... } ],
  "totalCount": 42,
  "page": 1,
  "pageSize": 20
}
```

### POST /api/partners — Body
```json
{
  "name": "Acme Lda",
  "taxNumber": "123456789",
  "type": "CUSTOMER",
  "email": "contact@acme.ao",
  "creditLimit": 5000.00
}
```

### PUT /api/partners/{id} — Body
```json
{
  "name": "Acme Updated",
  "taxNumber": "987654321",
  "type": "SUPPLIER",
  "email": "new@acme.ao",
  "creditLimit": 7500.00,
  "isActive": false,
  "version": 1
}
```

> O campo `version` é obrigatório para concorrência otimista. Se não corresponder à versão atual, devolve **409 Conflict**.

---

## Códigos de erro

| Código | Exceção | Cenário |
|--------|---------|---------|
| 400 | `ArgumentException` | Validação de input |
| 404 | `KeyNotFoundException` | Recurso não encontrado |
| 409 | `ConflictException` | TaxNumber duplicado ou versão desatualizada |
| 500 | (outras) | Erro interno |

---

## Como executar

### Pré-requisitos
- .NET 10 SDK
- SQL Server (local ou Docker)

### Passos
```bash
# 1. Restaurar dependências
dotnet restore Partners.slnx

# 2. Configurar connection string em Partners.Api/appsettings.json

# 3. Aplicar migrations
dotnet ef database update --project Partners.Infra.Data --startup-project Partners.Api

# 4. Executar a API
dotnet run --project Partners.Api

# 5. Abrir Swagger
# http://localhost:5000  (ou a porta indicada)
```

---

## Testes

12 testes automatizados (9 unitários + 3 E2E).

```bash
# Correr todos
dotnet test Partners.slnx

# Apenas unitários
dotnet test Partners.Aplication.Tests
dotnet test Partners.Api.Tests

# Apenas E2E
dotnet test Partners.E2E.Tests
```

Ver `docs/comandos-testes.txt` e `docs/documentacao-testes.txt` para detalhes.


