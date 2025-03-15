namespace MsClean.Presentation;

using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

[ExcludeFromCodeCoverage]
public class FileUploadFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.RelativePath.Contains("File") && context.ApiDescription.HttpMethod == "POST")
        {

            var uploadFileMediaType = new OpenApiMediaType()
            {
                Schema = new OpenApiSchema()
                {
                    Type = "object",
                    Properties =
                        {
                            ["file"] = new OpenApiSchema()
                            {
                                Description = "Upload File",
                                Type = "file",
                                Format = "formData"
                            }
                        },
                    Required = new HashSet<string>() { "file" }
                }
            };

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = { ["multipart/form-data"] = uploadFileMediaType }
            };

        }
    }
}
