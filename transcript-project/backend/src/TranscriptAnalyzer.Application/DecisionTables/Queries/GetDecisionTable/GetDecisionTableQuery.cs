using MediatR;
using TranscriptAnalyzer.Application.DecisionTables.DTOs;

namespace TranscriptAnalyzer.Application.DecisionTables.Queries.GetDecisionTable;

public record GetDecisionTableQuery(Guid Id) : IRequest<DecisionTableDetailDto?>;
