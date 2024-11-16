var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.TrainingTDDWithCleanArch_Presentation_MinimalAPI>("minimalapi")
    .WithReference(redis)
    .WaitFor(redis);

builder.AddProject<Projects.CleanArchitectureSampleProject_Presentation_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();


