using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PPiChallenge.API.Middlewares;
using PPiChallenge.Core.Interfaces;
using PPiChallenge.Core.Interfaces.Repository;
using PPiChallenge.Core.Interfaces.Services;
using PPiChallenge.Infrastructure.DataBaseIttion;
using PPiChallenge.Infrastructure.Interfaces;
using PPiChallenge.Infrastructure.Interfaces.Repository;
using PPiChallenge.Infrastructure.Interfaces.Services;
using Serilog;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Configuración
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

#region Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICuentaRepository, CuentaRepository>();
builder.Services.AddScoped<IOrdenRepository, OrdenRepository>();
builder.Services.AddScoped<IActivoFinancieroRepository, ActivoFinancieroRepository>();
builder.Services.AddScoped<IEstadoOrdenRepository, EstadoOrdenRepository>();
#endregion

//┌──────────────┐
//│   Fatal      │ ← nivel más alto (más importante)
//│   Error      │
//│   Warning    │
//│   Info       │ ← valor típico en producción
//│   Debug      │
//│   Verbose    │ ← nivel más bajo (más detallado)
//└──────────────┘
//// Configurar Serilog con appsettings
#region Logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();
#endregion

//Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

#region Authentication & Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        ),
        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
        ClockSkew = TimeSpan.Zero,
        RequireExpirationTime = true
    };
});

builder.Services.AddAuthorization();
#endregion

#region Custom Services
builder.Services.AddHttpClient();
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<IOrdenService, OrdenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
#endregion

var app = builder.Build();

//Registro el middleware de las excepcione !!
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
