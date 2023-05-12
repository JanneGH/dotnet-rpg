global using AutoMapper;
global using dotnet_rpg.Models;
global using dotnet_rpg.Services.CharacterService;
global using Microsoft.EntityFrameworkCore;

using dotnet_rpg.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Make the API know they need to use the CharacterService class 
// whenever the Controller wants to inject the ICharacterService
// DI: If you want to change CharacterService and use another implementation class,
// simply change CharacterService and all controllers injecting ICharacterService will follow.

// AddScoped: create a new instance of the requested service for every request that comes in. 
// AddTransient: create a new instance to every Controller and every Service even within the same request
// AddSingleton creates one instance that is used for every request.

builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
