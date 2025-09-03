namespace PPiChallenge.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Recurso no encontrado");
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await WriteErrorAsync(context, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Acceso no autorizado");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized; //401
                await WriteErrorAsync(context, ex.Message);
            }
            catch (ForbiddenAccessException ex)
            {
                _logger.LogWarning(ex, "Acceso prohibido");
                context.Response.StatusCode = StatusCodes.Status403Forbidden; //403
                await WriteErrorAsync(context, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Operación invalida");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await WriteErrorAsync(context, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno del servidor");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await WriteErrorAsync(context, "Error interno del servidor");
            }
        }

        private static async Task WriteErrorAsync(HttpContext context, string message)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = message });
        }
    }

    //Custom Excepcion para 403
    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException(string message) : base(message) { }
    }
}
