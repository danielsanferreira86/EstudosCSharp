## [0.1.1] - 2026-11-26

### Adicionado

- Arquivo `global.json` para padronização do SDK .NET.
- Identificação dinâmica da versão da aplicação no Dashboard.
- Identificação dinâmica do framework em execução.

### Alterado

- Migração do .NET 9 para o .NET 10 LTS.
- Atualização do ambiente de desenvolvimento para Visual Studio 2026 Community.
- Padronização do SDK na versão 10.0.404, permitindo atualizações de patch compatíveis.
- Atualização da documentação técnica do projeto.

### Segurança

- Revisão dos pacotes NuGet e suas dependências.
- Manutenção das versões estáveis dos pacotes.
- Validação dos mecanismos de autenticação e autorização.

### Homologação

- Compilação realizada com sucesso.
- 14 verificações de regressão aprovadas.
- Validação da comunicação com SQL Server Express.
- Validação da integração com a API oficial do IBGE.
- Validação das operações do cadastro de Estados.
- Preservação das regras de negócio e da integridade referencial.

### Infraestrutura

- Provisionamento do Azure App Service Plan F1.
- Criação do Azure App Service com runtime .NET 10 LTS.
- Preparação da infraestrutura para a primeira publicação da aplicação.