## Project to Manage Courses in my System


![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)  \
Used .NET, ASP.NET Core and EF Core to develop the solution.

![Postgres](https://img.shields.io/badge/postgres-%23316192.svg?style=for-the-badge&logo=postgresql&logoColor=white) \
Used Postgres as the database.

![TailwindCSS](https://img.shields.io/badge/tailwindcss-%2338B2AC.svg?style=for-the-badge&logo=tailwind-css&logoColor=white) \
Used TailwindCSS for CSS styling

![Docker](https://img.shields.io/badge/docker-%230db7ed.svg?style=for-the-badge&logo=docker&logoColor=white) \
Used Docker Container to easier CI/CD and deployments.

![ElasticSearch](https://img.shields.io/badge/-ElasticSearch-005571?style=for-the-badge&logo=elasticsearch) \
Used ElasticSearch APM server and ElasticSearch for logging and distributed tracing.

![Fedora](https://img.shields.io/badge/Fedora-294172?style=for-the-badge&logo=fedora&logoColor=white) \
Developed on Fedora Operating System

![GitHub](https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white)\
Used Github Actions for CI

#### In this repository I have implemented following technologies
1. Object Mapping with Mapster
2. Entity Framework Core 6 with Postgres
3. dotnet command line application with DI and Configuration
4. Basic ASP.NET Core Integration Test Solution
5. MediatR
6. Serilog logging to file and console
7. Health Checks


#### TODO
- [x] ~~Implement Health Check UI to see which components are down.~~
- [ ] Implement Vue/Vite frontend with ASP.NET core SPA template
- [ ] Implement ```dotnet-suggest```
- [ ] Implement Linting withing IDE and third party tools
- [ ] Integrate with Nautilus Scripts

Courses.CLI linux command
```bash
    find . -maxdepth 1 -type d | tail -n +2 | xargs -{} readlink -f '{}' |  xargs -I{} courses add '{}' --categories vue
```

Command to update dotnet-tool in directory Courses.CLI
```bash
    dotnet pack
    dotnet tool update --global --add-source ./nupkg courses.cli
```

Command to add migration for course module. [SO Link](https://stackoverflow.com/a/39621455)
```bash
dotnet ef migrations add <migration-name> -s API -p CourseModule
```

Command to update database for course module
```bash
dotnet ef database update -s API -p CourseModule
```

Tags:
1. Documentation
    XML Comments / Open API
2. Security
3. Observability
4. Developer Experience
5. Test / Testability
6. Performance / Scaling
7. Maintainability / Technical Debt
8. Cost Optimization