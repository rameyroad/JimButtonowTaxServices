using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TranscriptAnalyzer.Application.Common.Behaviors;

namespace TranscriptAnalyzer.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // AutoMapper
        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(assembly);
        }, assembly);

        // FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
