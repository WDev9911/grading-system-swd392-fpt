using SWD392.GradingTool.Domain.Entities;

namespace SWD392.GradingTool.Application.Interfaces;

public interface IClassGroupRepository
{
    Task<ClassGroup> AddAsync(ClassGroup classGroup, CancellationToken cancellationToken = default);
    Task<ClassGroup?> GetByClassNameAsync(string className, CancellationToken cancellationToken = default);
    Task<ClassGroup?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ClassGroup>> GetAllWithStudentsAsync(CancellationToken cancellationToken = default);
}
