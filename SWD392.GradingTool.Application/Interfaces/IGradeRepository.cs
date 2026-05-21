using SWD392.GradingTool.Domain.Entities;

namespace SWD392.GradingTool.Application.Interfaces;

public interface IGradeRepository
{
    Task<Grade?> GetByStudentAndRubricAsync(int studentId, int rubricId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Grade>> GetGradesByStudentAsync(int studentId, CancellationToken cancellationToken = default);
    Task<Grade> AddAsync(Grade grade, CancellationToken cancellationToken = default);
    Task UpdateAsync(Grade grade, CancellationToken cancellationToken = default);
    Task<IEnumerable<Grade>> GetGradesByClassAsync(int classId, CancellationToken cancellationToken = default);
}
