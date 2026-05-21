using SWD392.GradingTool.Domain.Entities;

namespace SWD392.GradingTool.Application.Interfaces;

public interface IRubricRepository
{
    /// <summary>
    /// Lưu danh sách Rubric vào database, trả về danh sách đã có Id.
    /// </summary>
    Task<IEnumerable<Rubric>> AddRangeAsync(IEnumerable<Rubric> rubrics, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lưu 1 rubric vào database, trả về entity đã có Id.
    /// </summary>
    Task<Rubric> AddAsync(Rubric rubric, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kiểm tra duplicate RubricName trong DB. Trả về danh sách tên đã tồn tại.
    /// </summary>
    Task<IEnumerable<string>> GetDuplicateNamesAsync(IEnumerable<string> rubricNames, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy toàn bộ rubric.
    /// </summary>
    Task<IEnumerable<Rubric>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<Rubric>> GetByIdsAsync(IEnumerable<int> rubricIds, CancellationToken cancellationToken = default);
}