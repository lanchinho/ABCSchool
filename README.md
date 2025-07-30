# ABCSchool

Este projeto foi desenvolvido durante o curso **Advanced .NET FullStack: Multi-Tenant Applications (NEW)** ministrado pelo professor **Junior Matlou**.

## Descri��o

O objetivo deste projeto � demonstrar a implementa��o de uma aplica��o multi-tenant utilizando .NET 8, Entity Framework Core, e outras tecnologias modernas do ecossistema .NET. O projeto aborda pr�ticas avan�adas de arquitetura, autentica��o, autoriza��o, e isolamento de dados.

## Depend�ncias Principais

Abaixo est�o listadas as principais depend�ncias utilizadas neste projeto:

- **Finbuckle.MultiTenant** (8.0.0): Suporte a multi-tenancy em aplica��es .NET.
- **Finbuckle.MultiTenant.AspNetCore** (8.0.0): Integra��o multi-tenant com ASP.NET Core.
- **Finbuckle.MultiTenant.EntityFrameworkCore** (8.0.0): Suporte multi-tenant para Entity Framework Core.
- **Microsoft.AspNetCore.Authentication.JwtBearer** (8.0.18): Autentica��o JWT.
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore** (8.0.18): Identidade baseada em Entity Framework Core.
- **Microsoft.EntityFrameworkCore.SqlServer** (8.0.18): Provedor SQL Server para EF Core.
- **Microsoft.EntityFrameworkCore.Tools** (8.0.18): Ferramentas para EF Core.
- **NSwag.AspNetCore** (14.4.0): Gera��o de documenta��o Swagger/OpenAPI.
- **Swashbuckle.AspNetCore** (6.6.2): Integra��o Swagger para ASP.NET Core.
- **Mapster** (7.4.0): Mapeamento de objetos.
- **MediatR** (13.0.0): Implementa��o do padr�o Mediator.
- **FluentValidation** (12.0.0): Valida��o de modelos.
- **ABCShared.Library** (1.0.10): Biblioteca compartilhada do curso.

## Estrutura do Projeto

- **WebApi**: Camada de apresenta��o (API REST).
- **Infrastructure**: Implementa��o de persist�ncia, identidade, e servi�os multi-tenant.
- **Application**: Camada de aplica��o, regras de neg�cio e handlers.
- **Domain**: Entidades e modelos de dom�nio.

## Observa��es

Este projeto � exclusivamente educacional e foi desenvolvido como parte das atividades do curso. Para mais informa��es sobre o curso e o professor, consulte a plataforma oficial ou entre em contato com o instrutor.