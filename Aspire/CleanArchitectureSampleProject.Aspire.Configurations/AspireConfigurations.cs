namespace CleanArchitectureSampleProject.Aspire.Configurations;

public static class AspireConfigurations
{
    public static class ProjectNames
    {
        public static string MinimalApi => "minimalapi";
        public static string ControllerApi => "controllerapi";
        public static string BlazorApp => "blazorapp";
        public static string DatabaseMigrator => "cleanarchitecturesampleproject-service-databasemigration";


        public static string ConsoleApp1 => "consoleapp1";
    }

    public static class Services
    {
        public static string RedisCacheName => "cache";
        public static string PostgresServerName => "dbserver";
        public static string PostgresDatabaseName => "dbproducts";
        public static string PostgresContainerVolume => "cleanarchitecturesampleproject-db-volume";
        public static string AzureStorageName => "storage";
        public static string AzureQueueName => "queues";
    }
}
