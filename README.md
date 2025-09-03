# PPiChallenge.API
Solucion .Net 8 Web Api Rest, en arquitectura Limpia, desarrollada por Mario Leal Fuentes.
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
      Responsabilidad: Implementa los servicios y la persistencia de datos (aqui esta el DbContext de Entity Framework).
      Esta capa depende de Core (porque implementa las interfaces definidas allí).
# General: 
      Tiene implementado Serilog con ILogger<T> con tres nieveles implementados para error debug e informacion
         Tiene tres appSeting para develop, testin y produccion
