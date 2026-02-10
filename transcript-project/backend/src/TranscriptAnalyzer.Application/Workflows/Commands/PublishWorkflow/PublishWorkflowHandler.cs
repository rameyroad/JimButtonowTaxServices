using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.Workflows.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Workflows.Commands.PublishWorkflow;

public class PublishWorkflowHandler : IRequestHandler<PublishWorkflowCommand, WorkflowDefinitionDetailDto?>
{
    private readonly DbContext _dbContext;
    private readonly ITenantContext _tenantContext;
    private readonly IMapper _mapper;

    public PublishWorkflowHandler(
        DbContext dbContext,
        ITenantContext tenantContext,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
        _mapper = mapper;
    }

    public async Task<WorkflowDefinitionDetailDto?> Handle(
        PublishWorkflowCommand request,
        CancellationToken cancellationToken)
    {
        var workflow = await _dbContext.Set<WorkflowDefinition>()
            .Include(wd => wd.Steps)
            .FirstOrDefaultAsync(wd => wd.Id == request.Id, cancellationToken);

        if (workflow is null)
            return null;

        var userId = _tenantContext.UserId
            ?? throw new UnauthorizedAccessException("User context is required.");

        workflow.Publish(userId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<WorkflowDefinitionDetailDto>(workflow);
    }
}
