using Microsoft.EntityFrameworkCore;
using SWD392.GradingTool.Application.Interfaces;
using SWD392.GradingTool.Domain.Entities;
using SWD392.GradingTool.Infrastructure.Data;

namespace SWD392.GradingTool.Infrastructure.Repositories;

public class RubricRepository : IRubricRepository
{
    private readonly AppDbContext _context;

    public RubricRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Rubric>> AddRangeAsync(IEnumerable<Rubric> rubrics, CancellationToken cancellationToken = default)
    {
        await _context.Rubrics.AddRangeAsync(rubrics, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return rubrics;
    }

    public async Task<Rubric> AddAsync(Rubric rubric, CancellationToken cancellationToken = default)
    {
        await _context.Rubrics.AddAsync(rubric, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return rubric;
    }

    public async Task<IEnumerable<string>> GetDuplicateNamesAsync(IEnumerable<string> rubricNames, CancellationToken cancellationToken = default)
    {
        var namesList = rubricNames.ToList();

        // EF Core 8 hỗ trợ translation cho Contains trên danh sách local
        var duplicates = await _context.Rubrics
            .Where(r => namesList.Contains(r.RubricName))
            .Select(r => r.RubricName)
            .ToListAsync(cancellationToken);

        return duplicates;
    }

    public async Task<IEnumerable<Rubric>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Rubrics
            .OrderBy(r => r.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Rubric>> GetByIdsAsync(IEnumerable<int> rubricIds, CancellationToken cancellationToken = default)
    {
        var idList = rubricIds.ToList();
        return await _context.Rubrics
            .Where(r => idList.Contains(r.Id))
            .ToListAsync(cancellationToken);
    }
}