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
var dbServer = builder.AddPostgres(Services.PostgresServerName, port: 5433)
    .WithLifetime(ContainerLifetime.Persistent);
var db = dbServer.AddDatabase(Services.PostgresDatabaseName);
dbServer.WithDataVolume(Services.PostgresContainerVolume)
    .WithPgAdmin();
// PostgresDB

// Auth PostgresDB
var dbAuthServer = builder.AddPostgres(Services.PostgresServerAuthenticationName, port: 5434)
    .WithLifetime(ContainerLifetime.Persistent);
var dbAuth = dbAuthServer.AddDatabase(Services.PostgresDatabaseAuthenticationName);
dbAuthServer.WithDataVolume(Services.PostgresContainerAuthenticationVolume)
    .WithPgAdmin();
// Auth PostgresDB

var dbMigrator = builder.AddProject<CleanArchitectureSampleProject_Aspire_Service_DatabaseMigration>(ProjectNames.DatabaseMigratorApp)
    .WithReference(dbServer)
    .WithReference(dbAuthServer)
    .WithReference(dbAuth)
    .WithReference(db)
    .WithReference(queue)
    .WaitFor(dbServer)
    .WaitFor(dbAuthServer)
    .WaitFor(db)
    .WaitFor(dbAuth)
    .WaitFor(queue);

var authenticationApi = builder.AddProject<CleanArchitectureSampleProject_Presentation_Authentication>(ProjectNames.AuthenticationApp)
    .WithReference(dbAuthServer)
    .WithReference(dbAuth)
    .WaitFor(dbAuthServer)
    .WaitFor(dbAuth)
    .WaitFor(dbMigrator);

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

builder.AddProject<CleanArchitectureSampleProject_Presentation_Web>(ProjectNames.BlazorApp)
    .WithExternalHttpEndpoints()
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(minimalApi)
    .WaitFor(minimalApi);

builder.AddProject<CleanArchitectureSampleProject_Presentation_Worker>(ProjectNames.MessageWorkerApp)
    .WithReference(queue)
    .WaitFor(queue)
    .WithReference(minimalApi)
    .WaitFor(minimalApi);

builder.Build().Run();


