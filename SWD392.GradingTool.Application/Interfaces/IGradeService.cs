using SWD392.GradingTool.Application.DTOs;

namespace SWD392.GradingTool.Application.Interfaces;

public interface IGradeService
{
    Task<GradeResponse> GradeStudentAsync(GradeStudentRequest request, CancellationToken cancellationToken = default);
    Task<ClassGradeResponse> GetGradesByClassAsync(int classId, CancellationToken cancellationToken = default);
    Task<StudentGradeDetailResponse> GetStudentGradeDetailsAsync(int studentId, CancellationToken cancellationToken = default);
    Task<ExportFileResult> ExportClassGradesAsync(int classId, CancellationToken cancellationToken = default);
}