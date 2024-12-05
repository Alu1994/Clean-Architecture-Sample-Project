### Initialize DB
dotnet ef migrations add InitializeAuthDb --startup-project ..\..\Presentation\CleanArchitectureSampleProject.Presentation.Authentication

dotnet ef database update --startup-project ..\..\Presentation\CleanArchitectureSampleProject.Presentation.Authentication