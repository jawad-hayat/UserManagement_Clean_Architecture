using Data.Context;
using Domain.Models;
using IoC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Models.ConfigModels;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//ConfigurationManager configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


//Jwt Configuration Settings
var secret = builder.Configuration.GetSection("JwtConfig").GetValue<string>("Secret");

var jwtConfig = new JwtConfig
{
    Secret = secret,
    Audience = builder.Configuration.GetSection("JwtConfig").GetValue<string>("Audience"),
    ExpirationTime = builder.Configuration.GetSection("JwtConfig").GetValue<string>("ExpirationTime"),
    Issuer = builder.Configuration.GetSection("JwtConfig").GetValue<string>("Issuer"),
    RefreshTokenExpiration = builder.Configuration.GetSection("JwtConfig").GetValue<string>("RefreshTokenExpiration")
};
builder.Services.AddSingleton(jwtConfig);

var key = !string.IsNullOrEmpty(secret) ? Encoding.ASCII.GetBytes(secret) : null;
var tokenValidationParams = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    RequireExpirationTime = false,
    ClockSkew = TimeSpan.Zero, 
};

builder.Services.AddSingleton(tokenValidationParams);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = tokenValidationParams;
});

var emailConfig = new EmailConfig
{
    Mail = builder.Configuration.GetSection("EmailConfig").GetValue<string>("Mail"),
    Password = builder.Configuration.GetSection("EmailConfig").GetValue<string>("Password"),
    DisplayName = builder.Configuration.GetSection("EmailConfig").GetValue<string>("DisplayName"),
    Port = builder.Configuration.GetSection("EmailConfig").GetValue<int>("Port"),
    Host = builder.Configuration.GetSection("EmailConfig").GetValue<string>("Host"),
};
builder.Services.AddSingleton(emailConfig);

DependencyContainer.RegisterServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Open");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
