# AutoCore-DMS

Sistema web de gestão de concessionárias de veículos (Dealer Management System), desenvolvido como projeto de estudo, aprimoramento técnico e portfólio profissional.

## Objetivo

Desenvolver uma aplicação empresarial modular, aplicando conceitos de arquitetura de software, segurança, integração de sistemas, banco de dados e desenvolvimento web com C# e ASP.NET Core.

## Tecnologias utilizadas

- C#
- .NET 9
- ASP.NET Core Razor Pages
- SQL Server
- ADO.NET
- BCrypt
- HTML5
- CSS3
- JavaScript
- Bootstrap
- Git e GitHub
- API oficial do IBGE

## Arquitetura

O projeto utiliza uma arquitetura em camadas, com separação de responsabilidades entre:

- Models
- DTOs
- ViewModels
- Repositories
- Services
- Data
- Security
- Razor Pages

As páginas são organizadas por módulos funcionais, permitindo a evolução gradual do sistema.

## Funcionalidades implementadas

### Autenticação e segurança

- Autenticação baseada em cookies.
- Armazenamento seguro de senhas utilizando BCrypt.
- Autorização baseada em políticas.
- Controle de acesso por perfil de usuário.
- Proteção antiforgery.
- Validação de dados no servidor.
- Comandos SQL parametrizados.

### Interface

- Layout responsivo.
- Tema escuro personalizado.
- Menu de navegação modular.
- Componentes visuais reutilizáveis.
- Design System compartilhado.

### Cadastro de Estados

- Listagem de unidades federativas.
- Pesquisa e paginação.
- Indicadores de registros.
- Inclusão, alteração e exclusão.
- Validação de duplicidades.
- Controle de integridade referencial.
- Integração com a API oficial do IBGE.
- Preenchimento automático dos dados oficiais.
- Cache de consultas externas.
- Tratamento de indisponibilidade do serviço externo.

## Roadmap

- [x] Estruturação do banco de dados
- [x] Models e Repositories iniciais
- [x] Serviço de autenticação
- [x] Página de Login
- [x] Autorização e controle de permissões
- [x] Layout principal e navegação
- [x] Cadastro de Estados
- [ ] Cadastro de Cidades
- [ ] Cadastro de Usuários
- [ ] Cadastro de Clientes
- [ ] Cadastro de Veículos
- [ ] Módulo Comercial
- [ ] Módulo de Estoque
- [ ] Módulo de Oficina
- [ ] Módulo Financeiro
- [ ] Módulo Fiscal
- [ ] Relatórios gerenciais

## Qualidade e segurança

O cadastro de Estados foi submetido a testes funcionais, de integração, segurança, integridade referencial, responsividade e regressão.

A documentação de homologação encontra-se no diretório `docs/testes`.

## Configuração local

Para executar o projeto, é necessário possuir:

- SDK do .NET 9.
- SQL Server ou SQL Server Express.
- Visual Studio ou outra IDE compatível.
- Banco de dados configurado.
- Connection string definida por meio de User Secrets ou variáveis de ambiente.

Credenciais e configurações sensíveis não são distribuídas no repositório.

## Status

Projeto em desenvolvimento contínuo, com entregas incrementais por módulo.

## Autor

Daniel S. Ferreira

Programador e Analista de Sistemas.

Projeto desenvolvido para aprimoramento técnico em C#, ASP.NET Core e tecnologias relacionadas ao desenvolvimento de sistemas empresariais.