# ABCSchool

Este projeto foi desenvolvido durante o curso **Advanced .NET FullStack: Multi-Tenant Applications (NEW)** ministrado pelo professor **Junior Matlou**.

## Descrição

O objetivo deste projeto é demonstrar a implementação de uma aplicação multi-tenant utilizando .NET 8, Entity Framework Core, e outras tecnologias modernas do ecossistema .NET. O projeto aborda práticas avançadas de arquitetura, autenticação, autorização, e isolamento de dados.

## Dependências Principais

Abaixo estão listadas as principais dependências utilizadas neste projeto:

- **Finbuckle.MultiTenant** (8.0.0): Suporte a multi-tenancy em aplicações .NET.
- **Finbuckle.MultiTenant.AspNetCore** (8.0.0): Integração multi-tenant com ASP.NET Core.
- **Finbuckle.MultiTenant.EntityFrameworkCore** (8.0.0): Suporte multi-tenant para Entity Framework Core.
- **Microsoft.AspNetCore.Authentication.JwtBearer** (8.0.18): Autenticação JWT.
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore** (8.0.18): Identidade baseada em Entity Framework Core.
- **Microsoft.EntityFrameworkCore.SqlServer** (8.0.18): Provedor SQL Server para EF Core.
- **Microsoft.EntityFrameworkCore.Tools** (8.0.18): Ferramentas para EF Core.
- **NSwag.AspNetCore** (14.4.0): Geração de documentação Swagger/OpenAPI.
- **Swashbuckle.AspNetCore** (6.6.2): Integração Swagger para ASP.NET Core.
- **Mapster** (7.4.0): Mapeamento de objetos.
- **MediatR** (13.0.0): Implementação do padrão Mediator.
- **FluentValidation** (12.0.0): Validação de modelos.
- **ABCShared.Library** (1.0.10): Biblioteca compartilhada do curso.

## Estrutura do Projeto

- **WebApi**: Camada de apresentação (API REST).
- **Infrastructure**: Implementação de persistência, identidade, e serviços multi-tenant.
- **Application**: Camada de aplicação, regras de negócio e handlers.
- **Domain**: Entidades e modelos de domínio.

## Observações

Este projeto é exclusivamente educacional e foi desenvolvido como parte das atividades do curso. Para mais informações sobre o curso e o professor, consulte a plataforma oficial ou entre em contato com o instrutor.