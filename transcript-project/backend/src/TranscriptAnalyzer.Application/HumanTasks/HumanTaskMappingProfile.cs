using AutoMapper;
using TranscriptAnalyzer.Application.HumanTasks.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.HumanTasks;

public class HumanTaskMappingProfile : Profile
{
    public HumanTaskMappingProfile()
    {
        CreateMap<HumanTask, HumanTaskListItemDto>();
        CreateMap<HumanTask, HumanTaskDetailDto>();
    }
}
