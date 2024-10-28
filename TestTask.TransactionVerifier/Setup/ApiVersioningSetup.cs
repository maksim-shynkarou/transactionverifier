namespace TestTask.TransactionVerifier.WebApi.Setup;

public static class ApiVersioningSetup
{
    /// <summary>
    /// Add API versioning.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddDefaultApiVersioning(this IServiceCollection services)
    {
        services
            .AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = false;
            })
            .AddVersionedApiExplorer(o =>
            {
                o.GroupNameFormat = "'v'VVV";
                o.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}
