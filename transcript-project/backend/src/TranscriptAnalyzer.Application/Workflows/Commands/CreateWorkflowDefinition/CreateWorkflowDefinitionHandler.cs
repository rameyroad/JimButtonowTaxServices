using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Workflows.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Workflows.Commands.CreateWorkflowDefinition;

public class CreateWorkflowDefinitionHandler : IRequestHandler<CreateWorkflowDefinitionCommand, WorkflowDefinitionDetailDto>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public CreateWorkflowDefinitionHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<WorkflowDefinitionDetailDto> Handle(
        CreateWorkflowDefinitionCommand request,
        CancellationToken cancellationToken)
    {
        var workflow = new WorkflowDefinition(request.Name, request.Description, request.Category);

        _dbContext.Set<WorkflowDefinition>().Add(workflow);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var result = await _dbContext.Set<WorkflowDefinition>()
            .Include(wd => wd.Steps)
            .FirstAsync(wd => wd.Id == workflow.Id, cancellationToken);

        return _mapper.Map<WorkflowDefinitionDetailDto>(result);
    }
}
