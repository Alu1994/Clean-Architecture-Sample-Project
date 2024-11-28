# Clean Architecture Sample Project

The main objective of this project is to revisit the principles of Clean Architecture, Testing and some new features of .NET.


### Initialize DB
dotnet ef migrations add InitializeAuthDb --startup-project ..\..\Presentation\CleanArchitectureSampleProject.Presentation.Authentication

dotnet ef database update --startup-project ..\..\Presentation\CleanArchitectureSampleProject.Presentation.Authentication