var builder = DistributedApplication.CreateBuilder(args);

// Redis Cache
var redis = builder.AddRedis(Services.RedisCacheName);
// Redis Cache

// PostgresDB
var dbServer = builder.AddPostgres(Services.PostgresServerName);
var db = dbServer.AddDatabase(Services.PostgresDatabaseName);
dbServer.WithDataVolume(Services.PostgresContainerVolume)
    .WithPgAdmin();
// PostgresDB

var dbMigrator = builder.AddProject<CleanArchitectureSampleProject_Aspire_Service_DatabaseMigration>(ProjectNames.DatabaseMigrator)
    .WithReference(dbServer)
    .WithReference(db)
    .WaitFor(dbServer)
    .WaitFor(db);

var controllerApi = builder.AddProject<CleanArchitectureSampleProject_Presentation_ControllerAPI>(ProjectNames.ControllerApi)
    .WithReference(db)
    .WaitFor(db)
    .WithReference(redis)
    .WaitFor(redis)
    .WaitFor(dbMigrator);

var minimalApi = builder.AddProject<CleanArchitectureSampleProject_Presentation_MinimalAPI>(ProjectNames.MinimalApi)
    .WithReference(db)
    .WaitFor(db)
    .WithReference(redis)
    .WaitFor(redis)
    .WaitFor(dbMigrator);

builder.AddProject<CleanArchitectureSampleProject_Presentation_Web>(ProjectNames.BlazorApp)
    .WithExternalHttpEndpoints()
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(minimalApi)
    .WaitFor(minimalApi);

builder.Build().Run();


