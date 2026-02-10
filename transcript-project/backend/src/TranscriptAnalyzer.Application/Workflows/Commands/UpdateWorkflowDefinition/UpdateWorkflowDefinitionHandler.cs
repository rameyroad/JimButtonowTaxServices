using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Workflows.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Workflows.Commands.UpdateWorkflowDefinition;

public class UpdateWorkflowDefinitionHandler : IRequestHandler<UpdateWorkflowDefinitionCommand, WorkflowDefinitionDetailDto?>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateWorkflowDefinitionHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<WorkflowDefinitionDetailDto?> Handle(
        UpdateWorkflowDefinitionCommand request,
        CancellationToken cancellationToken)
    {
        var workflow = await _dbContext.Set<WorkflowDefinition>()
            .Include(wd => wd.Steps)
            .FirstOrDefaultAsync(wd => wd.Id == request.Id, cancellationToken);

        if (workflow is null)
            return null;

        if (request.Name is not null)
            workflow.UpdateName(request.Name);

        if (request.Description is not null)
            workflow.UpdateDescription(request.Description);

        if (request.Category is not null)
            workflow.UpdateCategory(request.Category);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<WorkflowDefinitionDetailDto>(workflow);
    }
}
