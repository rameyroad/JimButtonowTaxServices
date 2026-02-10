using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TranscriptAnalyzer.Application.Common.Behaviors;
using TranscriptAnalyzer.Application.Workflows.Services;
using TranscriptAnalyzer.Domain.Enums;

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

        // Workflow engine
        services.AddScoped<DecisionTableStepExecutor>();
        services.AddScoped<IEnumerable<KeyValuePair<WorkflowStepType, IStepExecutor>>>(sp =>
            new List<KeyValuePair<WorkflowStepType, IStepExecutor>>
            {
                new(WorkflowStepType.DecisionTable, sp.GetRequiredService<DecisionTableStepExecutor>()),
            });
        services.AddScoped<IWorkflowEngine, WorkflowEngine>();

        return services;
    }
}
