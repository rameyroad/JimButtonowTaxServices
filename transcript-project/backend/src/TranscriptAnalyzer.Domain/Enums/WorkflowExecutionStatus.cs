namespace TranscriptAnalyzer.Domain.Enums;

public enum WorkflowExecutionStatus
{
    NotStarted,
    Running,
    Paused,
    Completed,
    Failed,
    Cancelled
}
