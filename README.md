# \# Partners API

# 

# API RESTful desenvolvida em .NET para a gestão de parceiros de negócio (clientes, fornecedores, etc.), construída com princípios de Clean Architecture, Injeção de Dependências (IoC) e Entity Framework Core.

# 

# \---

# 

# \## Arquitetura do Projeto

# 

# O projeto está organizado em camadas para garantir o desacoplamento, testabilidade e facilidade de manutenção:

# 

# \- Partners.Api: Camada de apresentação contendo os Controllers, middlewares e configurações da API.

# \- Partners.Aplication: Contém as regras de negócio da aplicação, DTOs (CreatePartnerDto, UpdatePartnerDto, PartnerDetailsDto), Mappers e Interfaces/Implementações dos Serviços (IPartnerService).

# \- Partners.Domain: Camada central do sistema. Contém as Entidades do domínio (Partner), Enums (PartnerType) e Interfaces dos Repositórios (IPartnerRepository).

# \- Partners.Infra.Data: Implementação do acesso a dados, mapeamentos do EF Core, DbContext e repositórios.

# \- Partners.Infra.Ioc: Contentor de Injeção de Dependências encarregue de registar os serviços e repositórios no Program.cs.

# 

# \---

# 

# \## Tecnologias e Ferramentas

# 

# \- .NET SDK (C#)

# \- Entity Framework Core (Acesso a dados e Migrations)

# \- OpenAPI / Swagger (Documentação interativa da API)

# \- SQL Server (Base de dados relacional)

# 

# \---

# 

# \## Endpoints da API (/api/partners)

# 

# A API expõe as seguintes rotas RESTful para a entidade Partners:

# 

# \- GET /api/partners: Obtém a lista completa de parceiros (200 OK)

# \- GET /api/partners/{id}: Obtém os detalhes de um parceiro por ID (200 OK / 404 Not Found)

# \- POST /api/partners: Regista um novo parceiro (201 Created / 400 Bad Request)

# \- PUT /api/partners/{id}: Atualiza os dados de um parceiro existente (204 No Content / 404 Not Found)

# \- DELETE /api/partners/{id}: Elimina um parceiro por ID (204 No Content / 404 Not Found)

# 

# \---

# 

# \## Como Executar o Projeto

# 

# \### Pré-requisitos

# \- .NET SDK

# \- Instância do SQL Server a correr localmente ou num contentor Docker.

# 

# \### Passo a Passo

# 1\. Clonar o repositório e aceder à pasta do projeto.

# 2\. Configurar a Connection String no appsettings.json da pasta Partners.Api.

# 3\. Aplicar as Migrations:

# &#x20;  dotnet ef database update --project Partners.Infra.Data --startup-project Partners.Api

# 4\. Executar a API:

# &#x20;  dotnet run --project Partners.Api

# 5\. Aceder ao Swagger no navegador para testar os endpoints.

