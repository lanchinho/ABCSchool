using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Reflection;

namespace Infrastructure.OpenApi;

public class SwaggerGlobalAuthProcessor(string scheme) : IOperationProcessor
{
    private readonly string _scheme = scheme;
    public SwaggerGlobalAuthProcessor()
        : this(JwtBearerDefaults.AuthenticationScheme) { }

    /// <summary>
    /// Processes the given operation to conditionally apply security requiremnts for Swagger documentation.
    /// </summary>
    /// <param name="context">The context containing operation and API metadata.</param>
    /// <returns>
    /// <c>true</c> if the processing is successful; otherwise, <c>false</c>
    /// The processor checks for the presence of the <see cref="AllowAnonymousAttribute"/>
    /// in the endpoint's metadata. If present, it skips adding security requirements.
    /// If not present and no security requirements exist, it  adds a security requirement
    /// for the specified authentication scheme.
    /// </returns>
    public bool Process(OperationProcessorContext context)
    {
        var list = ((AspNetCoreOperationProcessorContext)context)
            .ApiDescription.ActionDescriptor.TryGetPropertyValue<List<object>>("EndpointMetadata");

        if (list is not null)
        {
            if (list.OfType<AllowAnonymousAttribute>().Any())
                return true;

            if (context.OperationDescription.Operation.Security.Count == 0)
            {
                (context.OperationDescription.Operation.Security ??= [])
                    .Add(new OpenApiSecurityRequirement
                    {
                        {
                            _scheme,
                            Array.Empty<string>()
                        }
                    });
            }
        }
        return true;
    }
}

public static class ObjectExtensions
{
    public static T TryGetPropertyValue<T>(this object obj, string propertyName, T defaultValue = default)
        => obj.GetType().GetRuntimeProperty(propertyName) is PropertyInfo propertyInfo
              ? (T)propertyInfo.GetValue(obj) : default;
}
