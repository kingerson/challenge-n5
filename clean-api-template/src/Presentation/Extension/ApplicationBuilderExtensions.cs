namespace MsClean.Presentation.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using MsClean.Domain;

[ExcludeFromCodeCoverage]
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        _ = app.UseExceptionHandler(appError => appError.Run(async context =>
        {
            var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

            if (contextFeature != null)
            {
                var statusCode = contextFeature.Error switch
                {
                    ValidationException ex => HttpStatusCode.BadRequest,
                    MsCleanException ex => HttpStatusCode.BadRequest,
                    _ => HttpStatusCode.InternalServerError
                };

                var apiError = new ApiError([contextFeature.Error.Message], contextFeature.Error.InnerException?.Message, contextFeature.Error.StackTrace);

                if (contextFeature.Error.GetType() == typeof(MsCleanException))
                {
                    var innerException = contextFeature.Error.InnerException as FluentValidation.ValidationException;
                    var providerException = (MsCleanException)contextFeature.Error;
                    if (innerException is not null)
                    {
                        var errors = ((FluentValidation.ValidationException)contextFeature.Error.InnerException).Errors.Select(x => x.ErrorMessage).ToArray();
                        apiError.Messages = errors;
                    }
                }

                context.Response.StatusCode = (int)statusCode;
                context.Response.ContentType = "application/json";


                await context.Response.WriteAsync(JsonSerializer.Serialize(apiError));
            }
        }));

        return app;
    }
}
