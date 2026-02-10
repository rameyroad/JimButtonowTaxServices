using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TranscriptAnalyzer.Application.Common.Behaviors;
using TranscriptAnalyzer.Application.Formulas.Services;
using TranscriptAnalyzer.Application.HumanTasks.Services;
using TranscriptAnalyzer.Application.Issues.Services;
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

        // Workflow engine + step executors
        services.AddScoped<DecisionTableStepExecutor>();
        services.AddScoped<CalculationStepExecutor>();
        services.AddScoped<TranscriptAnalysisStepExecutor>();
        services.AddScoped<HumanTaskStepExecutor>();
        services.AddScoped<IEnumerable<KeyValuePair<WorkflowStepType, IStepExecutor>>>(sp =>
            new List<KeyValuePair<WorkflowStepType, IStepExecutor>>
            {
                new(WorkflowStepType.DecisionTable, sp.GetRequiredService<DecisionTableStepExecutor>()),
                new(WorkflowStepType.Calculation, sp.GetRequiredService<CalculationStepExecutor>()),
                new(WorkflowStepType.HumanTask, sp.GetRequiredService<HumanTaskStepExecutor>()),
            });
        services.AddScoped<IWorkflowEngine, WorkflowEngine>();

        return services;
    }
}
