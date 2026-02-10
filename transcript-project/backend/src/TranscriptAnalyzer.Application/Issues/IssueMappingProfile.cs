using AutoMapper;
using TranscriptAnalyzer.Application.Issues.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Issues;

public class IssueMappingProfile : Profile
{
    public IssueMappingProfile()
    {
        CreateMap<Issue, IssueListItemDto>();
        CreateMap<Issue, IssueDetailDto>();
    }
}
