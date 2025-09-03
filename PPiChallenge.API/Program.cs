using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

//Configuracion: Agrego explicitamente lo siguiente
//(Esto ya está incluido por defecto en el template de ASP.NET Core)
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
//FIN: Configuracion: Agrego explicitamente lo siguiente
//(Esto ya está incluido por defecto en el template de ASP.NET Core)

#region ENTITY FRAMEWORK
//Agregar DbContext al contenedor de servicios
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add the necessary using directive for SQL Server support in Entity Framework Core


// Ensure the following line is present in your .csproj file to include the required package:
// <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="x.x.x" />

// No changes to the existing code are needed beyond ensuring the above using directive is added.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICuentaRepository, CuentaRepository>();
builder.Services.AddScoped<IOrdenRepository, OrdenRepository>();
builder.Services.AddScoped<IActivoFinancieroRepository, ActivoFinancieroRepository>();

#endregion ENTITY FRAMEWORK
/* Logs Levels
┌──────────────┐
│   Fatal      │ ← nivel más alto (más importante)
│   Error      │
│   Warning    │
│   Info       │ ← valor típico en producción
│   Debug      │
│   Verbose    │ ← nivel más bajo (más detallado)
└──────────────┘
*/
// Configurar Serilog con appsettings
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();
// FIN Configurar Serilog con appsettings

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});


//Authentications
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
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
//Fin Authentications


//Here Adds Customers Services
builder.Services.AddHttpClient();
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<IOrdenService, OrdenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
//END of Here Adds Customers Services

var app = builder.Build();

//Registro el middleware de las excepcione !!
app.UseMiddleware<ExceptionMiddleware>();

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
