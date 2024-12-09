# Clean Architecture Sample Project

The main objective of this project is to revisit the principles of Clean Architecture, Testing and some new features of .NET.

### PGADMIN
docker run -p 5050:80 --name pgadmin \
    -e 'PGADMIN_DEFAULT_EMAIL=user@domain.com' \
    -e 'PGADMIN_DEFAULT_PASSWORD=SuperSecret' \
    -d dpage/pgadmin4

### PostgresDB	
docker run --name default-postgres -e POSTGRES_PASSWORD=mysecretpassword -d postgres

### Initialize DB
dotnet ef migrations add InitializeAuthDb --startup-project ..\..\Presentation\CleanArchitectureSampleProject.Presentation.Authentication

dotnet ef database update --startup-project ..\..\Presentation\CleanArchitectureSampleProject.Presentation.Authentication
