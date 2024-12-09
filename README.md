# Clean Architecture Sample Project

The main objective of this project is to revisit the principles of Clean Architecture, Testing and some new features of .NET.

### PGADMIN
docker run --name pgadmin -p 5050:80 -e 'PGADMIN_CONFIG_SERVER_MODE=False' -e 'PGADMIN_DEFAULT_EMAIL=email@email.com' -e 'PGADMIN_DEFAULT_PASSWORD=123456' --detach dpage/pgadmin4

### Initialize DB
dotnet ef migrations add InitializeAuthDb --startup-project ..\..\Presentation\CleanArchitectureSampleProject.Presentation.Authentication

dotnet ef database update --startup-project ..\..\Presentation\CleanArchitectureSampleProject.Presentation.Authentication
