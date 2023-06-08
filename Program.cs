global using AutoMapper;
global using dotnet_rpg.Models;
global using dotnet_rpg.Services.CharacterService;
global using Microsoft.EntityFrameworkCore;

using dotnet_rpg.Data;
using dotnet_rpg.Services.FightService;
using dotnet_rpg.Services.WeaponService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddControllers();

/// JwtBearerdefaults is a seperate NuGet
// Add authentication scheme
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // initialize new instance of token validation parameters & set them
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            // New symmetric security key that gets the encoded appsettings token value
            /// To handle the null warning the null forgiving operator is used (! at Value). 
            /// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-forgiving
            /// It tells the compiler the value will be there so a warning is not needed.
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("Appsettings:Token").Value!)),
            ValidateIssuer = false, // TODO find out why it is not needed
            ValidateAudience = false // TODO find out why it is not needed
        };
    });

/// register so when HttpContextAccessor is injected the API knows what to do with it.
builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Added the package Swashbuckle.AspNetCore.Filters to enable configuration
builder.Services.AddSwaggerGen(configuration =>
{   // add security definition to enable an option in the Swagger UI to enter the Bearer token
    // results in the Authorize button in Swagger :)
    configuration.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = """ Standard Authorization header using the bearer scheme. Example: "bearer {token}" """, /// .NET updated string handeling with """ so no escaping is needed anymore.
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    configuration.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAutoMapper(typeof(Program).Assembly);

/// Make the API know they need to use the CharacterService class 
/// whenever the Controller wants to inject the ICharacterService
/// DI: If you want to change CharacterService and use another implementation class,
/// simply change CharacterService and all controllers injecting ICharacterService will follow.

// AddScoped: create a new instance of the requested service for every request that comes in. 
/// AddTransient: create a new instance to every Controller and every Service even within the same request
/// AddSingleton creates one instance that is used for every request.

// Register the services
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IWeaponService, WeaponService>();
builder.Services.AddScoped<IFightService, FightService>();

// Register repository
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

var app = builder.Build();

// configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

/// placement of Authentication is deliberate above Authorization.
/// add the dotnet core authentication middleware to the IApplicationBuilder to
// enable authentication capabilities
app.UseAuthentication(); /// enables the Authorize Attribute for controllers for example.
app.UseAuthorization();

app.MapControllers();

app.Run();
