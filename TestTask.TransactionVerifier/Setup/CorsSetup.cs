namespace TestTask.TransactionVerifier.WebApi.Setup;

public static class CorsSetup
{
    public static IServiceCollection AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
            options.AddPolicy("allow", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        return services;
    }

    public static IApplicationBuilder UseCustomCors(this IApplicationBuilder app)
    {
        app.UseCors("allow");

        return app;
    }
}
