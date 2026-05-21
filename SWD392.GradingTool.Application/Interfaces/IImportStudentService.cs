using Microsoft.AspNetCore.Http;
using SWD392.GradingTool.Application.DTOs;

namespace SWD392.GradingTool.Application.Interfaces;

public interface IImportStudentService
{
    Task<ImportStudentResponseDto> ImportAsync(IFormFile file, CancellationToken cancellationToken = default);
}
