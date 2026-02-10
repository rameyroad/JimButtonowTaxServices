using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Workflows.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Workflows.Commands.UnpublishWorkflow;

public class UnpublishWorkflowHandler : IRequestHandler<UnpublishWorkflowCommand, WorkflowDefinitionDetailDto?>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public UnpublishWorkflowHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<WorkflowDefinitionDetailDto?> Handle(
        UnpublishWorkflowCommand request,
        CancellationToken cancellationToken)
    {
        var workflow = await _dbContext.Set<WorkflowDefinition>()
            .Include(wd => wd.Steps)
            .FirstOrDefaultAsync(wd => wd.Id == request.Id, cancellationToken);

        if (workflow is null)
            return null;

        workflow.Unpublish();
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<WorkflowDefinitionDetailDto>(workflow);
    }
}
