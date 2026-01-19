
using Formation_Ecommerce_11_2025.API.Models;
using System.Net;
using System.Text.Json;

namespace Formation_Ecommerce_11_2025.API.Middleware
{
    public class GlobalExceptionMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                // ➡ Passe la main au suivant (Controller, ou autre Middleware)
                await next(context);
            }
            catch (Exception ex)
            {
                // 🛑 Si une erreur "remonte" (n'a pas été gérée plus bas), on l'attrape ici.
                await HandleExceptionAsync(context, ex);
            }
        }

        // Transforme l'exception C# technique (ex: NullReferenceException) en réponse HTTP standardisée (400, 404, 500)
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message) = exception switch
            {
                KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message), // 404
                InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message), // 400
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, exception.Message), // 401
                _ => (HttpStatusCode.InternalServerError, "Une erreur inattendue est survenue") // 500
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;


            // On utilise notre format standard ApiResponse pour que le client comprenne l'erreur
            var payload = new ApiResponse<object>
            {
                Success = false,
                Message = message,
                Errors = new List<string> { exception.Message }
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}

