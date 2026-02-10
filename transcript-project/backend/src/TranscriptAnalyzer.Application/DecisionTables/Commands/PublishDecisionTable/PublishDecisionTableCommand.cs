using MediatR;
using TranscriptAnalyzer.Application.DecisionTables.DTOs;

namespace TranscriptAnalyzer.Application.DecisionTables.Commands.PublishDecisionTable;

public record PublishDecisionTableCommand(Guid Id) : IRequest<DecisionTableDetailDto?>;
