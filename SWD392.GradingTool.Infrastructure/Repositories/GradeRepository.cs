using Microsoft.EntityFrameworkCore;
using SWD392.GradingTool.Application.Interfaces;
using SWD392.GradingTool.Domain.Entities;
using SWD392.GradingTool.Infrastructure.Data;

namespace SWD392.GradingTool.Infrastructure.Repositories;

public class GradeRepository : IGradeRepository
{
    private readonly AppDbContext _context;

    public GradeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Grade?> GetByStudentAndRubricAsync(int studentId, int rubricId, CancellationToken cancellationToken = default)
    {
        return await _context.Grades
            .FirstOrDefaultAsync(g => g.StudentId == studentId && g.RubricId == rubricId, cancellationToken);
    }

    public async Task<IEnumerable<Grade>> GetGradesByStudentAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _context.Grades
            .Include(g => g.Rubric)
            .Where(g => g.StudentId == studentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Grade> AddAsync(Grade grade, CancellationToken cancellationToken = default)
    {
        await _context.Grades.AddAsync(grade, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return grade;
    }

    public async Task UpdateAsync(Grade grade, CancellationToken cancellationToken = default)
    {
        grade.UpdatedDate = DateTime.UtcNow;
        _context.Grades.Update(grade);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Grade>> GetGradesByClassAsync(int classId, CancellationToken cancellationToken = default)
    {
        return await _context.Grades
            .Include(g => g.Rubric)
            .Include(g => g.Student)
            .Where(g => g.Student.ClassGroupId == classId)
            .ToListAsync(cancellationToken);
    }
}
