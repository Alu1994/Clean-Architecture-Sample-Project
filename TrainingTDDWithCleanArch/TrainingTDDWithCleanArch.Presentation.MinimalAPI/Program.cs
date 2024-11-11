using TrainingTDDWithCleanArch.Application;
using TrainingTDDWithCleanArch.Domain;
using TrainingTDDWithCleanArch.Repository;
using TrainingTDDWithCleanArch.Presentation.MinimalAPI.Endpoints.Products;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =========== Add Layer Dependency Injection ===========
builder.Services.AddDomainLayer();
builder.Services.AddApplicationLayer();
builder.Services.AddRepositoryLayer();
// =========== Add Layer Dependency Injection ===========

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// =========== Add Endpoints ===========
app.MapProducts();
// =========== Add Endpoints ===========

app.Run();