using AutoMapper;
using TranscriptAnalyzer.Application.Clients.DTOs;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;
using TranscriptAnalyzer.Domain.ValueObjects;

namespace TranscriptAnalyzer.Application.Clients;

public class ClientMappingProfile : Profile
{
    public ClientMappingProfile()
    {
        // Address mappings
        CreateMap<Address, AddressDto>();
        CreateMap<AddressDto, Address>()
            .ConstructUsing(src => new Address(
                src.Street1,
                src.Street2,
                src.City,
                src.State,
                src.PostalCode,
                src.Country));

        // Client to DTOs
        CreateMap<Client, ClientListItemDto>()
            .ForMember(dest => dest.TaxIdentifierMasked, opt => opt.MapFrom(src => FormatMaskedTaxIdentifier(src.ClientType, src.TaxIdentifierLast4)))
            .ForMember(dest => dest.IsArchived, opt => opt.MapFrom(src => src.DeletedAt != null));

        CreateMap<Client, ClientDto>()
            .ForMember(dest => dest.TaxIdentifierMasked, opt => opt.MapFrom(src => FormatMaskedTaxIdentifier(src.ClientType, src.TaxIdentifierLast4)))
            .ForMember(dest => dest.IsArchived, opt => opt.MapFrom(src => src.DeletedAt != null));

        CreateMap<Client, ClientDetailDto>()
            .ForMember(dest => dest.TaxIdentifierMasked, opt => opt.MapFrom(src => FormatMaskedTaxIdentifier(src.ClientType, src.TaxIdentifierLast4)))
            .ForMember(dest => dest.IsArchived, opt => opt.MapFrom(src => src.DeletedAt != null))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy != null
                ? new UserSummaryDto(src.CreatedBy.Id, $"{src.CreatedBy.FirstName} {src.CreatedBy.LastName}", src.CreatedBy.Email)
                : null))
            .ForMember(dest => dest.ActiveAuthorizationCount, opt => opt.MapFrom(src =>
                src.Authorizations.Count(a => a.Status == AuthorizationStatus.Active)))
            .ForMember(dest => dest.TranscriptCount, opt => opt.MapFrom(src => src.Transcripts.Count));

        // User to UserSummaryDto
        CreateMap<User, UserSummaryDto>()
            .ConstructUsing(src => new UserSummaryDto(
                src.Id,
                $"{src.FirstName} {src.LastName}",
                src.Email));
    }

    private static string FormatMaskedTaxIdentifier(ClientType clientType, string last4)
    {
        return clientType == ClientType.Individual
            ? $"***-**-{last4}"   // SSN format: ***-**-1234
            : $"**-***{last4}";  // EIN format: **-***1234
    }
}
