using AutoMapper;
using TranscriptAnalyzer.Application.DecisionTables.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.DecisionTables;

public class DecisionTableMappingProfile : Profile
{
    public DecisionTableMappingProfile()
    {
        CreateMap<DecisionTable, DecisionTableListItemDto>()
            .ForMember(dest => dest.RuleCount, opt => opt.MapFrom(src => src.Rules.Count))
            .ForMember(dest => dest.ColumnCount, opt => opt.MapFrom(src => src.Columns.Count));

        CreateMap<DecisionTable, DecisionTableDetailDto>()
            .ForMember(dest => dest.Columns, opt => opt.MapFrom(src =>
                src.Columns.OrderBy(c => c.SortOrder).ToList()))
            .ForMember(dest => dest.Rules, opt => opt.MapFrom(src =>
                src.Rules.OrderBy(r => r.Priority).ToList()));

        CreateMap<DecisionTableColumn, DecisionTableColumnDto>();

        CreateMap<DecisionRule, DecisionRuleDto>()
            .ForMember(dest => dest.Conditions, opt => opt.MapFrom(src => src.Conditions.ToList()))
            .ForMember(dest => dest.Outputs, opt => opt.MapFrom(src => src.Outputs.ToList()));

        CreateMap<RuleCondition, RuleConditionDto>();

        CreateMap<RuleOutput, RuleOutputDto>();
    }
}
