using SWD392.GradingTool.Application.DTOs;

namespace SWD392.GradingTool.Application.Interfaces;

public interface IStudentService
{
    Task<IEnumerable<StudentDto>> GetByClassIdAsync(int classId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StudentSearchDto>> SearchAsync(string keyword, CancellationToken cancellationToken = default);
    Task<StudentSearchDto> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default);
}