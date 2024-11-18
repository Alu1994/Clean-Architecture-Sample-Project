using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client.Extensions.Msal;

var builder = DistributedApplication.CreateBuilder(args);

// Redis Cache
var redis = builder.AddRedis(Services.RedisCacheName);
// Redis Cache

var value = builder.AddParameter("value");

// Azure Queue
var storage = builder.AddAzureStorage(Services.AzureStorageName)
            .RunAsEmulator();
var queues = storage.AddQueues(Services.AzureQueueName);
// Azure Queue

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





builder.AddProject<ConsoleApp1>(ProjectNames.ConsoleApp1)
    .WithReference(queues)
    .WaitFor(queues);





builder.Build().Run();


