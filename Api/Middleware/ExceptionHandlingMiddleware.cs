using System.Net;
using System.Text.Json;

namespace Api.Middleware
{
    /// <summary>
    /// Middleware для глобальной обработки исключений
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Конструктор middleware обработки исключений
        /// </summary>
        /// <param name="next">Следующий middleware в конвейере</param>
        /// <param name="logger">Логгер для записи ошибок</param>
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Обрабатывает HTTP запрос и перехватывает исключения
        /// </summary>
        /// <param name="context">HTTP контекст</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла необработанная ошибка: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Обрабатывает исключение и возвращает структурированный ответ
        /// </summary>
        /// <param name="context">HTTP контекст</param>
        /// <param name="exception">Исключение для обработки</param>
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "Произошла внутренняя ошибка сервера",
                Details = exception.Message
            };

            // Определение типа ошибки и установка соответствующего статус-кода
            switch (exception)
            {
                case ArgumentException argEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Ошибка валидации данных";
                    response.Details = argEx.Message;
                    break;

                case InvalidOperationException invOpEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Некорректная операция";
                    response.Details = invOpEx.Message;
                    break;

                case KeyNotFoundException keyEx:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = "Ресурс не найден";
                    response.Details = keyEx.Message;
                    break;

                case UnauthorizedAccessException unauthEx:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = "Доступ запрещен";
                    response.Details = unauthEx.Message;
                    break;
            }

            context.Response.StatusCode = response.StatusCode;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var jsonResponse = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(jsonResponse);
        }
    }

    /// <summary>
    /// Модель ответа с ошибкой
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// HTTP статус-код
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Детали ошибки
        /// </summary>
        public string Details { get; set; } = string.Empty;

        /// <summary>
        /// Время возникновения ошибки
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

