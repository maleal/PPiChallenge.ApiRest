# PPiChallenge.API
Solución .Net 8 Web Api Rest, en arquitectura Limpia, desarrollada por Mario Leal Fuentes para Challenge PPi.

# Instalación y Ejecución de la Solución:
    a. Debe configurar el string de Conexión en el appsettings.json adecuado (hay tres: development, testing y producción),
    seteando a Source con el nombre de su host de BD y, si quiere, cambie el nombre de la DB en Catalog.
b. Abra la PMC y corra los comandos:
    Add-Migration <NombreDeLaMigracion>
    y una vez creada la migración:
    Update-Database
c. En la base de datos verá creadas las tablas y algunas seteadas con datos iniciales
   obtenidos desde el PDF del challenge.
d. Login y autenticación de una cuenta:
    El controlador AuthController tiene un endpoint de Login, que validará el usuario y password ingresados de la cuenta.
    Si está registrado e ingresó las credenciales correctas, le responderá con un TOKEN JWT para ser usado por el cliente en los siguientes requests.
    Tiene también otro endpoint para crear una cuenta, Y el tercero es para probar un request desde un Postman, por ejemplo,
    que tenga seteado en el Header el token JWT creado con el Login de la cuenta.
e. En el controlador OrdenController están las operaciones CRUD para las Órdenes.
   Estas se guardarán en la base de datos con los valores calculados según la solicitud del challenge.
   
# Esquema de la Web API – Gestión de Órdenes de Inversión -
La solución respeta los principios de Arquitectura Limpia, separando responsabilidades en distintas capas:
# PPiChallenge.Api
    Controllers
      AuthController.cs Expone los endpoints relacionados con autenticación y cuentas:
      Login → genera un JWT válido si las credenciales son correctas.
      RegistrarCuenta → registra un nuevo usuario en la base de datos.
      ProbarToken → endpoint protegido con [Authorize] para verificar que el JWT funciona.
      OrdenController.cs
    Middlewares
      ExceptionMiddleware.cs
      Middleware centralizado para capturar excepciones. Convierte errores en respuestas HTTP coherentes (401, 403, 404, 500, etc.), con   logging adecuado.
    Program.cs Configura los servicios de la aplicación:
      Inyección de dependencias.
      Configuración de autenticación con JWT (AddAuthentication, AddAuthorization).
      Configuración de Swagger con soporte para JWT.
      
# PPiChallenge.Core
      Responsabilidad: Contiene lógica y contratos de negocio independientes de cualquier infraestructura externa.
      DTOs, Entidades y las Interfaces para el patron Repository con UnitOfWork.
# PPiChallenge.Infrastructure
      Responsabilidad: Implementa los servicios y la persistencia de datos, 
      en la carpeta DataBaseIttion esta implementado ApplicationDbContext (el DbContext de Entity Framework).
      Esta capa depende de Core (porque implementa las interfaces definidas allí).
# General: 
      Tiene implementado Serilog con ILogger<T> con tres nieveles de logs para error debug e informacion
      Tiene tres appSettings.json para develop, testing y produccion.
