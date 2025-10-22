namespace FifaTracker.WebApi.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                var allowedOriginsEnv = configuration["ALLOWED_ORIGINS"];
                string[] allowedOrigins;

                if (!string.IsNullOrWhiteSpace(allowedOriginsEnv))
                {
                    allowedOrigins = allowedOriginsEnv
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(o => o.Trim())
                        .Where(o => !string.IsNullOrWhiteSpace(o))
                        .ToArray();
                }
                else
                {
                    allowedOrigins = configuration
                        .GetSection("AllowedOrigins")
                        .Get<string[]>()?.Where(o => !string.IsNullOrWhiteSpace(o)).ToArray()
                        ?? Array.Empty<string>();
                }

                if (allowedOrigins.Length > 0)
                {
                    Console.WriteLine($"🔒 CORS: Using specific origins: {string.Join(", ", allowedOrigins)}");
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                }
                else
                {
                    Console.WriteLine("⚠️ CORS: ALLOW ALL mode - no origins configured");
                    policy.SetIsOriginAllowed(origin => true)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                }
            });
        });

        return services;
    }
}