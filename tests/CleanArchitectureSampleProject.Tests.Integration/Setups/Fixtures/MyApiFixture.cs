namespace CleanArchitectureSampleProject.Tests.Integration.Setups.Fixtures;

using CleanArchitectureSampleProject.Infrastructure.Repository.Entities;
using CleanArchitectureSampleProject.Tests.Integration.Setups.Services;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

internal sealed class MyApiFixture
{
    internal static readonly MyApi Server = new();
    internal static readonly HttpClient Client = Server.CreateDefaultClient();

    internal static void Setup()
    {
        if (Client is null) return;
        Client.DefaultRequestHeaders.Clear();
    }

    internal sealed class MyApi : WebApplicationFactory<Program>
    {
        //Aqui vc tem acesso ao builder da sua Api original
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //Exemplo de como sobrecarregar o appsettings da sua api com o app settings de teste
            var integrationAppSettings = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.Tests.json", optional: false, reloadOnChange: true)
                .Build();

            builder.ConfigureAppConfiguration(config =>
            {
                config.AddConfiguration(integrationAppSettings);
            });

            builder.UseEnvironment("Tests").ConfigureTestServices(services => 
            {
                //Aqui temos acesso ao IServiceCollection da API nossa e podemos brincar com os serviços

                services.AddDbContext<ProductDataContext>(options =>
                    options.UseNpgsql("Host=localhost;Port=5441;Username=postgres;Password=dzJfU-Xv1rBzmckyTm05Cg;Database=dbproducts"));

                services.AddStackExchangeRedisCache(x =>
                {
                    x.Configuration = "localhost:6379";
                    x.InstanceName = "SellAPI";
                });

                services.AddMyApiMockedAuthentication();
            });
        }
    }
}
