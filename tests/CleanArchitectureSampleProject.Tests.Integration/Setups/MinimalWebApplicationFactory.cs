using CleanArchitectureSampleProject.Infrastructure.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Tests.Integration.Setups;

public class MinimalWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Tests");
        builder.ConfigureServices(services =>
        {
            services.AddDbContext<ProductDataContext>(options =>
                options.UseNpgsql("Host=localhost;Port=5441;Username=postgres;Password=dzJfU-Xv1rBzmckyTm05Cg;Database=dbproducts"));

            services.AddStackExchangeRedisCache(x =>
            {
                x.Configuration = "localhost:6379";
                x.InstanceName = "SellAPI";
            });
        });
    }
}