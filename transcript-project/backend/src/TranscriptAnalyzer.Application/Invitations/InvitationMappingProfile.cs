using AutoMapper;
using TranscriptAnalyzer.Application.Invitations.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Invitations;

public class InvitationMappingProfile : Profile
{
    public InvitationMappingProfile()
    {
        CreateMap<Invitation, InvitationDto>();
    }
}
