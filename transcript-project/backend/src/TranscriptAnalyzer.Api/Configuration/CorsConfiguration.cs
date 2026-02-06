namespace TranscriptAnalyzer.Api.Configuration;

public static class CorsConfiguration
{
    public const string PolicyName = "TranscriptAnalyzerPolicy";

    public static IServiceCollection AddCorsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                if (environment.IsDevelopment())
                {
                    policy
                        .WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
                else
                {
                    var allowedOrigins = configuration
                        .GetSection("Cors:AllowedOrigins")
                        .Get<string[]>() ?? [];

                    policy
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
                        .AllowCredentials();
                }
            });
        });

        return services;
    }

    public static IApplicationBuilder UseCorsConfiguration(this IApplicationBuilder app)
    {
        app.UseCors(PolicyName);
        return app;
    }
}
