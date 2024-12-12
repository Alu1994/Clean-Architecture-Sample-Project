namespace CleanArchitectureSampleProject.Aspire.Configurations;

public static class AspireConfigurations
{
    public static class ProjectNames
    {
        public static string AuthenticationApp => "authenticationapi";
        public static string MinimalApi => "minimalapi";
        public static string ControllerApi => "controllerapi";
        public static string BlazorApp => "blazorapp";
        public static string DatabaseMigratorApp => "dbmigratorworker";
        public static string MessageWorkerApp => "messageworkerapp";
    }

    public static class Services
    {
        public static string RedisCacheName => "cache";

        public static string PostgresServerName => "dbserver";
        public static string PostgresDatabaseName => "dbproducts";        
        public static string PostgresContainerVolume => "cleanarchitecturesampleproject-db-volume";

        public static string PostgresServerAuthenticationName => "dbauthserver";
        public static string PostgresDatabaseAuthenticationName => "dbauths";
        public static string PostgresContainerAuthenticationVolume => "db-auth-volume";


        public static string AzureStorageConnection => "storage";
        public static string AzureQueueConnection => "queues";
        public static string AzureQueueName => "dispatch-messaging";
    }
}
