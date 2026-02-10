using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.HumanTasks.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.HumanTasks.Queries.GetHumanTask;

public class GetHumanTaskHandler : IRequestHandler<GetHumanTaskQuery, HumanTaskDetailDto?>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public GetHumanTaskHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<HumanTaskDetailDto?> Handle(
        GetHumanTaskQuery request,
        CancellationToken cancellationToken)
    {
        var humanTask = await _dbContext.Set<HumanTask>()
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (humanTask is null)
            return null;

        return _mapper.Map<HumanTaskDetailDto>(humanTask);
    }
}
