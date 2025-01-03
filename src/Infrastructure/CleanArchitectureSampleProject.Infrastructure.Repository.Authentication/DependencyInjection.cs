﻿using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;
using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.Resources;
using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.Users;
using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.UsersResources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static CleanArchitectureSampleProject.Aspire.Configurations.AspireConfigurations;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Authentication;

public static class DependencyInjection
{
    public static IHostApplicationBuilder BuildAuthRepository(this IHostApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            // =========== Add EF PostgresDB - Aspire ===========
            builder.AddNpgsqlDbContext<AuthenticationDataContext>(Services.PostgresDatabaseAuthenticationName);
            // =========== Add EF PostgresDB - Aspire ===========
        }
        else
        {
            // Is not local env (Not .NET Aspire)
        }

        return builder;
    }

    public static IServiceCollection AddAuthRepositoryLayer(this IServiceCollection services)
    {                
        services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IResourceRepository, ResourceRepository>()
            .AddScoped<IUserResourceRepository, UserResourceRepository>();

        return services;
    }
}
