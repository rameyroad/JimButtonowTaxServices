using AutoMapper;
using TranscriptAnalyzer.Application.Workflows.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Workflows;

public class WorkflowMappingProfile : Profile
{
    public WorkflowMappingProfile()
    {
        CreateMap<WorkflowDefinition, WorkflowDefinitionListItemDto>()
            .ForMember(dest => dest.StepCount, opt => opt.MapFrom(src => src.Steps.Count));

        CreateMap<WorkflowDefinition, WorkflowDefinitionDetailDto>()
            .ForMember(dest => dest.Steps, opt => opt.MapFrom(src =>
                src.Steps.OrderBy(s => s.SortOrder).ToList()));

        CreateMap<WorkflowStep, WorkflowStepDto>();

        CreateMap<CaseWorkflow, CaseWorkflowDetailDto>()
            .ForMember(dest => dest.WorkflowName, opt => opt.MapFrom(src => src.WorkflowDefinition.Name))
            .ForMember(dest => dest.StepExecutions, opt => opt.MapFrom(src =>
                src.StepExecutions.OrderBy(se => se.CreatedAt).ToList()));

        CreateMap<CaseWorkflow, CaseWorkflowListItemDto>()
            .ForMember(dest => dest.WorkflowName, opt => opt.MapFrom(src => src.WorkflowDefinition.Name))
            .ForMember(dest => dest.TotalSteps, opt => opt.MapFrom(src => src.WorkflowDefinition.Steps.Count))
            .ForMember(dest => dest.CompletedSteps, opt => opt.MapFrom(src =>
                src.StepExecutions.Count(se => se.Status == Domain.Enums.StepExecutionStatus.Completed)));

        CreateMap<StepExecution, StepExecutionDto>()
            .ForMember(dest => dest.StepName, opt => opt.MapFrom(src => src.WorkflowStep.Name))
            .ForMember(dest => dest.StepType, opt => opt.MapFrom(src => src.WorkflowStep.StepType));
    }
}
