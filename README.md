🇵🇹 Português
CleanMadeira

O CleanMadeira é uma plataforma web desenvolvida para simplificar a gestão de Alojamentos Locais e empresas de limpeza. O objetivo do projeto é centralizar, numa única aplicação, a gestão de propriedades, planeamento de limpezas, manutenção, inventário e utilizadores, permitindo uma organização mais eficiente das operações do dia a dia.

Este projeto foi desenvolvido como um projeto pessoal com o objetivo de aprofundar conhecimentos em desenvolvimento de software utilizando ASP.NET Core MVC, Entity Framework Core e SQL Server, aplicando boas práticas como arquitetura em camadas, injeção de dependências e o padrão Repository.

Entre as principais funcionalidades encontram-se a autenticação e autorização de utilizadores através do ASP.NET Identity, gestão de propriedades, criação e atribuição de tarefas de limpeza, gestão de pedidos de manutenção, controlo de inventário, dashboard com indicadores operacionais e gestão de diferentes perfis de utilizador.

A aplicação foi desenvolvida recorrendo a ASP.NET Core MVC, C#, Entity Framework Core, SQL Server, Bootstrap 5, HTML, CSS e JavaScript, seguindo uma arquitetura em camadas composta pelos projetos Domain, Application, Infrastructure e Web, permitindo separar a lógica de negócio, acesso a dados e interface do utilizador de forma organizada e escalável.

O projeto encontra-se em desenvolvimento contínuo e pretende evoluir com novas funcionalidades, como notificações por email, API REST, aplicação móvel em .NET MAUI, relatórios estatísticos e melhorias na experiência do utilizador.

Este repositório representa o meu percurso de aprendizagem em desenvolvimento web com tecnologias Microsoft e pretende demonstrar competências em desenvolvimento full-stack, arquitetura de software, bases de dados relacionais e boas práticas de programação.


🇵🇹 Configuração

Antes de executar a aplicação, certifica-te de que tens instalado:

.NET 9 SDK (ou a versão utilizada pelo projeto)
SQL Server
Visual Studio 2022
Mailpit (para testes de envio de emails)

Depois de clonares o repositório:

Configura a connection string no ficheiro appsettings.json.
Inicia o Mailpit.

Executa as migrations:

Update-Database
Executa a aplicação.

Os emails enviados pela aplicação podem ser visualizados na interface do Mailpit.




🇬🇧 English
CleanMadeira

CleanMadeira is a web-based platform designed to simplify the management of Local Accommodation properties and cleaning companies. The goal of the project is to centralize property management, cleaning schedules, maintenance requests, inventory tracking, and user management into a single application, helping businesses organize their daily operations more efficiently.

This project was developed as a personal portfolio project to strengthen my knowledge of ASP.NET Core MVC, Entity Framework Core, and SQL Server, while applying software engineering best practices such as layered architecture, dependency injection, and the Repository pattern.

The application includes features such as user authentication and authorization with ASP.NET Identity, property management, cleaning task scheduling, maintenance request management, inventory control, operational dashboards, and role-based user management.

The project is built using ASP.NET Core MVC, C#, Entity Framework Core, SQL Server, Bootstrap 5, HTML, CSS, and JavaScript, following a layered architecture composed of Domain, Application, Infrastructure, and Web projects. This architecture keeps business logic, data access, and presentation clearly separated, making the application easier to maintain and extend.

The project is still under active development, with future plans including email notifications, a REST API, a .NET MAUI mobile application, reporting features, and additional improvements to the overall user experience.

This repository represents my learning journey as a software developer and showcases my skills in ASP.NET Core, full-stack web development, software architecture, relational databases, and modern software development practices.


🇬🇧 Setup

Before running the application, make sure you have installed:

.NET 9 SDK (or the version used by this project)
SQL Server
Visual Studio 2022
Mailpit (used for email testing)

After cloning the repository:

Configure the connection string in appsettings.json.
Start Mailpit.

Run the database migrations:

Update-Database
Start the application.

All emails sent by the application can be viewed through the Mailpit web interface.
