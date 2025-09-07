//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using PPiChallenge.API.Middlewares;
//using PPiChallenge.Core.Interfaces;
//using PPiChallenge.Core.Interfaces.Repository;
//using PPiChallenge.Core.Interfaces.Services;
//using PPiChallenge.Infrastructure.DataBaseIttion;
//using PPiChallenge.Infrastructure.Interfaces;
//using PPiChallenge.Infrastructure.Interfaces.Repository;
//using PPiChallenge.Infrastructure.Interfaces.Services;
//using Serilog;
//using System.IdentityModel.Tokens.Jwt;
//using System.Reflection;
//using System.Security.Claims;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

////Configuracion: Agrego explicitamente lo siguiente
////(Esto ya está incluido por defecto en el template de ASP.NET Core)
//builder.Configuration
//    .AddJsonFile("appsettings.json", optional: false)
//    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
//    .AddEnvironmentVariables();
////FIN: Configuracion: Agrego explicitamente lo siguiente
////(Esto ya está incluido por defecto en el template de ASP.NET Core)

//#region ENTITY FRAMEWORK
////Agregar DbContext al contenedor de servicios
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//// Add the necessary using directive for SQL Server support in Entity Framework Core


//// Ensure the following line is present in your .csproj file to include the required package:
//// <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="x.x.x" />

//// No changes to the existing code are needed beyond ensuring the above using directive is added.
//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped<ICuentaRepository, CuentaRepository>();
//builder.Services.AddScoped<IOrdenRepository, OrdenRepository>();
//builder.Services.AddScoped<IActivoFinancieroRepository, ActivoFinancieroRepository>();
//builder.Services.AddScoped<IEstadoOrdenRepository, EstadoOrdenRepository>();

//#endregion ENTITY FRAMEWORK
///* Logs Levels
//┌──────────────┐
//│   Fatal      │ ← nivel más alto (más importante)
//│   Error      │
//│   Warning    │
//│   Info       │ ← valor típico en producción
//│   Debug      │
//│   Verbose    │ ← nivel más bajo (más detallado)
//└──────────────┘
//*/
//// Configurar Serilog con appsettings
//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)
//    .Enrich.FromLogContext()
//    .CreateLogger();

//builder.Host.UseSerilog();
//// FIN Configurar Serilog con appsettings

//// Add services to the container.
//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
//    c.IncludeXmlComments(xmlPath);

//    //Para usar el JWT desde aqui (Swagger) en las pruebas del Token
//    //usando el boton Authorize verde arriba a la derecha.
//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });

//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            new string[] {}
//        }
//    });
//});


//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],//Con esto valido Usuario y
//        ValidAudience = builder.Configuration["Jwt:Audience"],//Audiencia con esta linea
//        IssuerSigningKey = new SymmetricSecurityKey(
//            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
//        ),
//        NameClaimType = ClaimTypes.Name,
//        ClockSkew = TimeSpan.Zero,
//        RequireExpirationTime = true,
//        ValidateActor = false,
//    };

//    options.Events = new JwtBearerEvents
//    {
//        OnAuthenticationFailed = context =>
//        {
//            Console.WriteLine($"JWT AUTH FAILED: {context.Exception.Message}");
//            Console.WriteLine($"JWT Exception: {context.Exception}");
//            Console.WriteLine($"JWT Issuer: '{builder.Configuration["Jwt:Issuer"]}'");
//            Console.WriteLine($"JWT Audience: '{builder.Configuration["Jwt:Audience"]}'");
//            Console.WriteLine($"JWT Key: '{builder.Configuration["Jwt:Key"]}'");
//            return Task.CompletedTask;
//        }
//    };
//});
//builder.Services.AddAuthorization();
////Fin Authentications


////Here Adds Customers Services
//builder.Services.AddHttpClient();
//builder.Services.AddScoped<IHttpService, HttpService>();
//builder.Services.AddScoped<IOrdenService, OrdenService>();
//builder.Services.AddScoped<IAuthService, AuthService>();
////END of Here Adds Customers Services

//var app = builder.Build();

////para la consola, sacar despues
//app.Use(async (context, next) =>
//{
//    var authHeader = context.Request.Headers["Authorization"];
//    Console.WriteLine($"$[DEBUG] Authorization Header: {authHeader}");
//    Console.WriteLine($"$[DEBUG] Path: {context.Request.Path}");
//    await next();
//    Console.WriteLine($"[DEBUG] Response Status: {context.Response.StatusCode}");
//});

////Registro el middleware de las excepcione !!
//app.UseMiddleware<ExceptionMiddleware>();

////Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();

//app.Run();


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
