using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.HumanTasks.DTOs;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.HumanTasks.Commands.ReassignHumanTask;

public class ReassignHumanTaskHandler : IRequestHandler<ReassignHumanTaskCommand, HumanTaskDetailDto?>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public ReassignHumanTaskHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<HumanTaskDetailDto?> Handle(
        ReassignHumanTaskCommand request,
        CancellationToken cancellationToken)
    {
        var humanTask = await _dbContext.Set<HumanTask>()
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (humanTask is null)
            return null;

        if (humanTask.Status == HumanTaskStatus.Completed)
            return _mapper.Map<HumanTaskDetailDto>(humanTask);

        humanTask.Reassign(request.AssignedToUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<HumanTaskDetailDto>(humanTask);
    }
}
