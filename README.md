# Overkill Docs

Editor colaborativo de documentos feito para fins de aprendizado e portfólio, construído com arquitetura limpa e princípios SOLID. O projeto foi projetado para operar em cenários de concorrência de recursos (edição colaborativa e alteração em tempo real), escalabilidade (execução em múltiplas instâncias) e baixo consumo de recursos para rodar em planos gratuitos em nuvem sem perda de funcionalidades.

O OverkillDocs tem recursos de criação de conta (dados pessoais e e-mail não são necessários, pois não é a finalidade do projeto), chat em tempo real e edição de documentos usando editor Markdown, inserção de imagens externas e vídeos do YouTube. A edição simultânea utiliza uma estratégia de bloqueio de fragmentos (locking) para garantir exclusividade durante a edição.

## Tecnologias usadas

- Front-end: Angular 20 utilizando RxJS + Signals, Angular Material para construção de telas.
- Back-end: ASP.NET com .NET 8, utilizando SignalR para a comunicação em tempo real via WebSocket.
- ORM: Entity Framework com suporte aos bancos SQL Server, PostgreSQL e SQLite.
- Cache: Redis para cache distribuído.
- Containers e Infraestrutura: Docker Compose para gerenciar os serviços, utilizando profiles para ativar e desativar recursos dinamicamente.
- Testes: Testes de integração usando xUnit e testes E2E (End-to-End) utilizando Playwright.
- CI/CD: GitHub Actions para execução automatizada dos testes e deploy automático na plataforma Render.

## Camadas da aplicação e decisões arquiteturais

### Overkilldocs.Web

É o front-end da aplicação feito com Angular Material. O site é responsivo e se adapta a telas mobile ou desktop sem que haja perda de recursos. Foi construído para ser reativo a alterações do usuário e a mudanças em tempo real via WebSocket. O sistema permite o gerenciamento de sessão de duas formas: compartilhada entre todas as abas (utilizando localStorage) ou isolada por aba (utilizando sessionStorage). Isso permite ao usuário manter múltiplas contas no mesmo navegador sem a necessidade de navegação privada.

Na implementação, a detecção automática de alterações (Zone.js) foi desativada e substituída por Signals para obter maior performance e previsibilidade. Para a edição de documentos, o navegador envia o fragmento após alguns segundos sem digitação do usuário, reduzindo o consumo de recursos em nuvem e o custo de operação do sistema.

A responsividade com SCSS foi implementada utilizando estilos isolados por faixas de breakpoints (mídias exclusivas). Em vez da abordagem tradicional mobile-first (onde propriedades de telas menores são herdadas e sobrescritas no desktop), optei por isolar as regras de cada tamanho de tela, o que impede a propagação indesejada de estilos entre dispositivos e facilita a manutenção, já que alterações em uma faixa específica não geram efeitos colaterais em outras.

### Overkilldocs.Api

Gerencia a comunicação HTTP e SignalR (WebSocket). Em vez de realizar um controle manual de status para cada requisição, optei por criar um _handler_ global que mapeia as exceções lançadas pelo código para seus respectivos status HTTP. As mensagens de erro são entregues seguindo o padrão **RFC 7807 (Problem Details)**.

Para o SignalR, existem dois recursos: chat e documentos. Optei por deixar ambos compartilhando a mesma conexão (HubConnection) para simplificar a implementação, reduzir latência de conexão e reduzir consumo de memória e processamento do servidor.

Na autenticação, em vez de utilizar JWT, preferi criar uma implementação própria de token baseada em identificadores únicos no padrão ULID. Essa abordagem traz dois grandes ganhos: permite realizar a revogação de sessões de forma simplificada (o que no JWT exigiria uma estrutura de blacklist) e evita a fragmentação de páginas de índices no banco de dados, já que o ULID possui uma ordenação cronológica em sua composição, ao contrário de hashes puramente aleatórios como o GUID.

### Overkilldocs.Core

É o coração do sistema. Todas as camadas se comunicam apenas com esta, e ela não depende de nenhuma outra. Optei por não dividir o projeto em camadas distintas de "Aplicação" e "Domínio" porque as regras de negócio são diretas e, para o escopo deste projeto, essa separação adicionaria uma complexidade desnecessária.

A estratégia de implementação dos serviços foi _fail-fast_: caso algum dado conflite com alguma regra, o serviço lançará uma exceção que será tratada pela API. Assim, a implementação dos métodos fica mais simples por não ter que lidar com fluxos internos de erro. Como o projeto usa Entity Framework, os serviços retornam apenas dados primitivos ou DTOs, e nunca as entidades do banco, evitando que alterações indesejadas reflitam no banco ou gerem efeitos colaterais.

### OverkillDocs.Infrastructure

Utiliza o padrão repositório como _fonte única da verdade_, isto é, todos os dados da aplicação virão de um só lugar. O uso do cache é gerenciado apenas por esta camada e não é exposto para o resto do sistema. A motivação disso é simplificar a implementação e evitar dados obsoletos que sejam esquecidos no cache.

Como a ideia do Entity Framework é ser agnóstico, não há implementações específicas para banco de dados aqui. A explicação sobre cada banco estará na seção _OverkillDocs.Migrator_.

O cache possui uma implementação dupla: usando Redis (externo) ou usando memória local (interno). A motivação para isso é que, em sistemas escaláveis, um cache global é necessário, mas em ambientes de baixa demanda ou poucos recursos, ter um servidor de cache é um ponto de falha a mais no sistema, além de gerar custos desnecessários. Toda a lógica de cache, assim como a geração das chaves, fica isolada dentro dos serviços de cache. Assim, evita-se ter diferentes padrões para gerar as chaves.

Nem todo repositório está relacionado a alguma entidade de banco; por exemplo, as mensagens do chat e o bloqueio de fragmentos de documentos ficam exclusivamente em cache e com tempo de vida definido. Assim, ao utilizar o repositório, não é necessário saber de onde o dado virá: ele será obtido do cache se estiver lá, ou do banco de dados quando for necessário.

O banco de dados e a estratégia de cache utilizada são configurados por variáveis de ambiente.

### OverkillDocs.Migrator

É composto por quatro projetos: um principal e um para cada banco de dados suportado. Ele serve apenas para configurar o banco usando autenticação de administrador, enquanto a aplicação normal utiliza um usuário com permissões limitadas. Ele é executado antes do _Overkilldocs.Api_.

As motivações para essa estrutura foram facilitar a manutenção — com cada projeto tendo sua responsabilidade exclusiva — e eliminar o risco de a migração ser executada simultaneamente ao subir múltiplas instâncias da API.

A escolha de suporte para cada banco de dados teve os seguintes critérios:

- SQLite: não exige um servidor, sendo apenas um arquivo local. É ideal para ambientes em nuvem gratuitos para fins de demonstração ou baixa demanda.
- SQL Server: banco corporativo amplamente utilizado no ecossistema .NET e em sistemas de grande porte.
- Postgres: alternativa leve ao SQL Server, sendo mais rápido para o ambiente de desenvolvimento e execução de testes.

## Docker e Testes

Devido à natureza customizável do ambiente, onde os recursos podem ser ativados ou desativados via `compose profiles`, os testes foram estruturados para validar os cenários principais da aplicação.

### Testes de integração

Os testes de integração utilizam **Testcontainers**, onde um container de banco de dados isolado é criado para cada teste, garantindo que os dados de uma execução não interfiram em outra. O banco utilizado nesses testes é o PostgreSQL, e o cache é mantido em memória (Memory Cache), sendo descartado a cada execução. Essa abordagem permite que os testes rodem em paralelo.

Os dados de entrada são gerados de forma aleatória com a biblioteca Bogus, e o log desses dados é registrado para facilitar o rastreamento caso algum teste falhe. A validação dos testes envolve checar o retorno da API, o estado do banco de dados e as chaves no cache.

### Testes End-to-End (E2E)

Desenvolvidos com **Playwright**, os testes E2E simulam a navegação real do usuário pelas telas. Eles utilizam PostgreSQL e Redis como infraestrutura. Para cada caso de teste é criado um usuário novo, mas os dados das execuções anteriores não são limpos — o ambiente é mantido "sujo" de propósito para simular o comportamento do sistema sob uso real.

Nos testes de chat e de CRUD de documentos, o Playwright abre duas instâncias de navegadores simultaneamente, configuradas com tamanhos de tela diferentes. Isso serve para avaliar o comportamento em tempo real no desktop e no mobile em cenários de concorrência e uso simultâneo.

### E os testes unitários?

Como as regras do serviço são bastante simples e a cobertura de código não é o objetivo, testes unitários no back-end são desnecessários aqui. Para o front-end, por causa da natureza assíncrona por design, os testes unitários não se mostraram funcionais e os E2E foram usados no lugar.

## Executando o projeto

Para executar o projeto localmente, é necessário ter o Docker Desktop instalado. Copie ou renomeie o arquivo `.env.example` para `.env` e faça as alterações comentadas nele. Em seguida, execute o script `start.ps1` e escolha a opção 1 para subir os containers. O site ficará disponível em http://localhost:80/.

### Alterando banco de dados

É necessário ter o Entity Framework instalado globalmente através do comando `dotnet tool install --global dotnet-ef`. As alterações no mapeamento são feitas no arquivo `OverkillDocs.Infrastructure/Data/AppDbContext.cs`. Depois, execute o script `add-migration.ps1` para gerar a migração para os três bancos de dados.

### Executando testes

Para os testes de integração, basta iniciá-los pelo Visual Studio com o Docker Desktop em execução.

Para os testes E2E, é necessário antes instalar o Playwright com o comando `dotnet tool install --global Microsoft.Playwright.CLI`.  Em seguida, execute o script `start.ps1` e escolha a opção 2 para subir os containers.

Caso deseje ver os testes sendo executados, há duas formas:

- Editando a propriedade "Headless" para _false_ em `OverkillDocs.Tests.E2E/Fixtures/PlaywrightFixture.cs`.
- Após executar os testes, acessando as gravações em `OverkillDocs.Tests.E2E/bin/Debug/net8.0/test-results`. A visualização passo a passo pode ser feita enviando os arquivos .zip gerados para https://trace.playwright.dev.

## Documentação e logs

Ao acessar o site, o link para a documentação do Swagger estará disponível na tela após o login. Os logs utilizam o servidor Seq (habilitado por variável de ambiente) e são estruturados com Serilog. Apesar de o Seq possuir padrões próprios, preferi usar o Serilog para que o formato do registro se mantenha idêntico caso outro provedor de logs seja usado no lugar dele. Ao executar o docker-compose, o Seq pode ser acessado localmente em http://localhost:5341/.
