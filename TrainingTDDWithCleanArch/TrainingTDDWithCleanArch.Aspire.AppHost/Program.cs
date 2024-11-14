var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TrainingTDDWithCleanArch_Presentation_MinimalAPI>("MinimalAPI");
builder.AddProject<Projects.TrainingTDDWithCleanArch_Presentation_API>("ControllerAPI");

builder.Build().Run();


