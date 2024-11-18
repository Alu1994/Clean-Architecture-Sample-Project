using Microsoft.Extensions.Hosting;
 
namespace ConsoleApp1;

public static class ListenerHostBuilder
{
    public static IHostApplicationBuilder Create(string[] args)
    {
        return Host.CreateApplicationBuilder(args);
    }
}