var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("cache");

builder.AddProject<Projects.TrainingTDDWithCleanArch_Presentation_MinimalAPI>("minimalapi")
    .WithExternalHttpEndpoints()
    .WithReference(redis)
    .WaitFor(redis);

builder.Build().Run();


