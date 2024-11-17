namespace CleanArchitectureSampleProject.Aspire.Configurations;

public static class AspireConfigurations
{
    public static class ProjectNames
    {
        public static string MinimalApi => "minimalapi";
        public static string ControllerApi => "controllerapi";
        public static string BlazorApp => "blazorapp";
        public static string DatabaseMigrator => "cleanarchitecturesampleproject-service-databasemigration";
    }

    public static class Services
    {
        public static string RedisCacheName => "cache";
        public static string PostgresServerName => "dbserver";
        public static string PostgresDatabaseName => "dbproducts";
        public static string PostgresContainerVolume => "cleanarchitecturesampleproject-db-volume";
    }
}
