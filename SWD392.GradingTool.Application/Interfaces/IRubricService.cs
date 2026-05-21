using SWD392.GradingTool.Application.DTOs;

namespace SWD392.GradingTool.Application.Interfaces;

public interface IRubricService
{
    Task<IEnumerable<RubricDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RubricDto> CreateAsync(CreateRubricRequest request, CancellationToken cancellationToken = default);
}