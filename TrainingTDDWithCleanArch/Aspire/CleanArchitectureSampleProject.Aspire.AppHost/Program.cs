var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("cache");

var controllerApi = builder.AddProject<Projects.CleanArchitectureSampleProject_Presentation_ControllerAPI>("controllerapi")
    .WithReference(redis)
    .WaitFor(redis);

var minimalApi = builder.AddProject<Projects.CleanArchitectureSampleProject_Presentation_MinimalAPI>("minimalapi")
    .WithReference(redis)
    .WaitFor(redis);

builder.AddProject<Projects.CleanArchitectureSampleProject_Presentation_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(minimalApi)
    .WaitFor(minimalApi);

builder.Build().Run();


