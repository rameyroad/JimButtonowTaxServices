using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Workflows.Services;

public interface IWorkflowEngine
{
    Task StartWorkflowAsync(CaseWorkflow caseWorkflow, CancellationToken cancellationToken = default);
    Task ResumeWorkflowAsync(CaseWorkflow caseWorkflow, CancellationToken cancellationToken = default);
}
