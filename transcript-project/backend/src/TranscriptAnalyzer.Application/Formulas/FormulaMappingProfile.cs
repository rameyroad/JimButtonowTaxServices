using AutoMapper;
using TranscriptAnalyzer.Application.Formulas.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Formulas;

public class FormulaMappingProfile : Profile
{
    public FormulaMappingProfile()
    {
        CreateMap<CalculationFormula, FormulaListItemDto>();
        CreateMap<CalculationFormula, FormulaDetailDto>();
    }
}
