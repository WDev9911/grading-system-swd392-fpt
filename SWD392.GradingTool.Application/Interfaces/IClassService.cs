using SWD392.GradingTool.Application.DTOs;

namespace SWD392.GradingTool.Application.Interfaces;

public interface IClassService
{
    Task<IEnumerable<ClassDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
