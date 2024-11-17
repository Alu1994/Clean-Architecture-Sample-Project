namespace CleanArchitectureSampleProject.Aspire.Configurations;

public static class AspireConfigurations
{
    public static class ProjectNames
    {
        public static string MinimalApi => "minimalapi";
        public static string ControllerApi => "controllerapi";
        public static string BlazorApp => "blazorapp";
    }

    public static string RedisCacheName => "cache";

    public static string PostgresName => "db";
    public static string PostgresDBName => "productsdb";
}
