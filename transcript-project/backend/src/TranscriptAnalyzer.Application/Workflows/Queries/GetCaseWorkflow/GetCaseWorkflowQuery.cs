using MediatR;
using TranscriptAnalyzer.Application.Workflows.DTOs;

namespace TranscriptAnalyzer.Application.Workflows.Queries.GetCaseWorkflow;

public record GetCaseWorkflowQuery(Guid ClientId, Guid CaseWorkflowId) : IRequest<CaseWorkflowDetailDto?>;
