# PPiChallenge.API - .NET 8 Web API

Solución .NET 8 Web API Rest en Arquitectura Limpia, desarrollada por **Mario Leal Fuentes** para Challenge PPi.

## Descripción

Web API para gestión de órdenes de inversión con autenticación JWT segura y arquitectura escalable basada en principios de Clean Architecture.

## Tecnologías Utilizadas

- **.NET 8** - Framework principal
- **Entity Framework Core 8** - ORM y gestión de base de datos
- **SQL Server** - Base de datos relacional
- **JWT** - Autenticación con tokens
- **BCrypt** - Hashing de contraseñas
- **Serilog** - Logging estructurado
- **Swagger/OpenAPI** - Documentación interactiva
- **MSTest** - Framework de testing

## Configuracion Base de Datos:
- ** Debe configurar el string de Conexión en el appsettings.json adecuado (hay tres: development, testing y producción),
    seteando a Source con el nombre de su host de BD y, si quiere, cambie el nombre de la DB en Catalog.
- ** Abra la PMC y corra los comandos:
    Add-Migration <NombreDeLaMigracion>
    y una vez creada la migración:
    Update-Database
- ** En la base de datos verá creadas las tablas y algunas seteadas con datos iniciales obtenidos desde el PDF del challenge.

# Seguridad en la Web Api:
##  Autenticación JWT

### a. Implementación de Login Seguro
He implementado un sistema de autenticación JWT robusto con:

- **Firma HMAC-SHA256** para tokens JWT
- **Clave secreta de 256+ bits** configurada en `appsettings.json`
- **BCrypt** con salting automático para hashing de contraseñas
- **Validación completa** de:
  -  Issuer (Emisor)
  -  Audience (Destinatario) 
  -  Lifetime (Tiempo de vida)
  -  SigningKey (Clave de firma)

**Nota:** La entidad `Cuenta` contiene los campos de usuario y password hasheado.

### b. Endpoint Protegido con JWT
- **Validación de credenciales** contra base de datos
- **Verificación de cuenta activa** antes de generar token
- **Generación de JWT** con claims personalizados
- **Protección de endpoints** con `[Authorize]`

#### Ejemplo de flujo:
** Ver en Swagger

## 🎯 Endpoints Principales

### 🔐 AuthController

| Método | Endpoint | Descripción | Autenticación |
|--------|----------|-------------|---------------|
| `POST` | `/api/auth/login` | Autenticación y generación de JWT | ❌ No requiere |
| `POST` | `/api/auth/registrar` | Registro de nuevas cuentas | ❌ No requiere |
| `GET` | `/api/auth/probar-token` | Verificación de token válido | ✅ Requiere JWT |

#### 📝 Ejemplo de Login
** Ver en Swagger

### 💰 OrdenController

| Método | Endpoint | Descripción | Autenticación |
|--------|----------|-------------|---------------|
| `GET` | `/api/orden` | Obtener todas las órdenes del sistema | ✅ Requiere JWT |
| `POST` | `/api/orden` | Crear nueva orden para activo financiero | ✅ Requiere JWT |
| `GET` | `/api/orden/porCuenta/{cuentaId}` | Obtener órdenes por ID de cuenta | ✅ Requiere JWT |
| `PATCH` | `/api/orden/{ordenId}/estado` | Actualizar estado de una orden | ✅ Requiere JWT |
| `DELETE` | `/api/orden/{ordenId}` | Eliminar una orden existente | ✅ Requiere JWT |

#### 📝 Ejemplo de Crear Orden
VerSWagger

# Esquema de la Web API – Gestión de Órdenes de Inversión -
La solución respeta los principios de Arquitectura Limpia, separando responsabilidades en distintas capas:
# PPiChallenge.UnitTestWithMSTestProject
    (comienzo enumerando este proyecto porque es un Opcional que me parecio importante hacerlo)
    Proyecto de UnitTest con MSTest, con los test necesarios para validar la creacion de las Ordenes, con los calculos pedidos por el challenge para diferentes tipos de Activos      financieros.
# PPiChallenge.Api
    Controllers
      AuthController.cs Expone los endpoints relacionados con autenticación y cuentas:
      Login → genera un JWT válido si las credenciales son correctas.
      RegistrarCuenta → registra un nuevo usuario en la base de datos.
      ProbarToken → endpoint protegido con [Authorize] para verificar que el JWT funciona. Nota: Para proteger todos los Endpoint solo se necesita agregar [Authorize]
      y estaran protegidos.
      OrdenController.cs Expone endPoints para las operaciones CRUD solicitadas por el Challenge. (Nota: No estan protegidas con [Authorize] por ahora.)
    Middlewares
      ExceptionMiddleware.cs
      Middleware centralizado para capturar excepciones. Convierte errores en respuestas HTTP coherentes (401, 403, 404, 500, etc.), con   logging adecuado.
    Program.cs Configura los servicios de la aplicación:
      Inyección de dependencias.
      Configuración de autenticación con JWT (AddAuthentication, AddAuthorization).
      Proximamente Configuración de Swagger con soporte para JWT para hacer pruebas directamente desde Swagger.
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
