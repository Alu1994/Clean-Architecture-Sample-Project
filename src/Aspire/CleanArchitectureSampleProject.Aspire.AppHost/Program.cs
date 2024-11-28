var builder = DistributedApplication.CreateBuilder(args);

// Redis Cache
var redis = builder.AddRedis(Services.RedisCacheName)
    .WithRedisInsight();
// Redis Cache

// Adds Azure Queue
var storage = builder.AddAzureStorage(Services.AzureStorageConnection).RunAsEmulator();
var queue = storage.AddQueues(Services.AzureQueueConnection);
// Adds Azure Queue

// PostgresDB
var dbServer = builder.AddPostgres(Services.PostgresServerName);
var db = dbServer.AddDatabase(Services.PostgresDatabaseName);
dbServer.WithDataVolume(Services.PostgresContainerVolume)
    .WithPgAdmin();
// PostgresDB

var authenticationApi = builder.AddProject<CleanArchitectureSampleProject_Presentation_Authentication>(ProjectNames.AuthenticationApp);

var dbMigrator = builder.AddProject<CleanArchitectureSampleProject_Aspire_Service_DatabaseMigration>(ProjectNames.DatabaseMigrator)
    .WithReference(dbServer)
    .WithReference(db)
    .WithReference(queue)
    .WaitFor(dbServer)
    .WaitFor(db)
    .WaitFor(queue);

var controllerApi = builder.AddProject<CleanArchitectureSampleProject_Presentation_ControllerAPI>(ProjectNames.ControllerApi)
    .WithReference(db)
    .WaitFor(db)
    .WithReference(redis)
    .WaitFor(redis)
    .WaitFor(dbMigrator);

var minimalApi = builder.AddProject<CleanArchitectureSampleProject_Presentation_MinimalAPI>(ProjectNames.MinimalApi)
    .WithReference(db)
    .WithReference(redis)
    .WithReference(queue)
    .WithReference(authenticationApi)
    .WaitFor(db)
    .WaitFor(redis)
    .WaitFor(dbMigrator)
    .WaitFor(queue)
    .WaitFor(authenticationApi);

var gRPCServer = builder.AddProject<CleanArchitectureSampleProject_Presentation_gRPC>(ProjectNames.gRPCServer)
    .WithReference(db)
    .WithReference(redis)
    .WithReference(queue)
    .WaitFor(db)
    .WaitFor(redis)
    .WaitFor(dbMigrator)
    .WaitFor(queue);

builder.AddProject<CleanArchitectureSampleProject_Presentation_Web>(ProjectNames.BlazorApp)
    .WithExternalHttpEndpoints()
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(minimalApi)
    .WaitFor(minimalApi)
    .WithReference(gRPCServer)
    .WaitFor(gRPCServer);

builder.Build().Run();


