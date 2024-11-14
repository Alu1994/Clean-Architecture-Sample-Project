namespace TrainingTDDWithCleanArch.Aspire.AppHost;

internal static class LoggingHelper
{
    internal static IResourceBuilder<T> WithSharedLoggingLevels<T>(this IResourceBuilder<T> builder) where T : IResourceWithEnvironment
    {
        var dict = new Dictionary<string, string>
        {
            { "Default", "Information" },
            { "System", "Warning" },
            { "Microsoft", "Warning" },
            { "Microsoft.Hosting", "Information" },
            { "Duende", "Warning" }
        };

        foreach (var item in dict.Keys)
        {
            builder = builder.WithEnvironment($"Logging__LogLevel__{item}", dict[item]);
        }
        return builder;
    }
}