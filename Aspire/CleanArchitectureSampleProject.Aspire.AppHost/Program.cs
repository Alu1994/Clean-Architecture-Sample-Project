//using CleanArchitectureSampleProject.Aspire.Configurations;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Redis Cache
var redis = builder.AddRedis("cache");
// Redis Cache

// PostgresDB
var dbServer = builder.AddPostgres("dbserver");
var db = dbServer.AddDatabase("dbproducts");

dbServer.WithDataVolume("cleanarchitecturesampleproject-db-volume")
    .WithPgAdmin();
// PostgresDB

var controllerApi = builder.AddProject<CleanArchitectureSampleProject_Presentation_ControllerAPI>("controllerapi")
    .WithReference(redis)
    .WaitFor(redis);

var minimalApi = builder.AddProject<CleanArchitectureSampleProject_Presentation_MinimalAPI>("minimalapi")
    .WithReference(db)
    //.WaitFor(db)
    .WithReference(redis);
    //.WaitFor(redis);

builder.AddProject<CleanArchitectureSampleProject_Presentation_Web>("blazorapp")
    .WithExternalHttpEndpoints()
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(minimalApi)
    .WaitFor(minimalApi);

builder.AddProject<CleanArchitectureSampleProject_Service_DatabaseMigration>("cleanarchitecturesampleproject-service-databasemigration")
    .WithReference(dbServer)
    .WithReference(db);

builder.Build().Run();


