using Microsoft.EntityFrameworkCore;
using SWD392.GradingTool.Application.Interfaces;
using SWD392.GradingTool.Domain.Entities;
using SWD392.GradingTool.Infrastructure.Data;

namespace SWD392.GradingTool.Infrastructure.Repositories;

public class ClassGroupRepository : IClassGroupRepository
{
    private readonly AppDbContext _context;

    public ClassGroupRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ClassGroup> AddAsync(ClassGroup classGroup, CancellationToken cancellationToken = default)
    {
        _context.ClassGroups.Add(classGroup);
        await _context.SaveChangesAsync(cancellationToken);
        return classGroup;
    }

    public async Task<ClassGroup?> GetByClassNameAsync(string className, CancellationToken cancellationToken = default)
    {
        return await _context.ClassGroups
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.ClassName == className, cancellationToken);
    }

    public async Task<ClassGroup?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.ClassGroups
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ClassGroup>> GetAllWithStudentsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ClassGroups
            .Include(g => g.Students)
            .OrderByDescending(g => g.CreatedDate)
            .ToListAsync(cancellationToken);
    }
}
