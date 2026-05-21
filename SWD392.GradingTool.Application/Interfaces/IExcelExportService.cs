using SWD392.GradingTool.Application.DTOs;

namespace SWD392.GradingTool.Application.Interfaces;

/// <summary>
/// Abstraction cho việc generate file Excel — implementation cụ thể dùng ClosedXML
/// nằm ở tầng Infrastructure để tách rời nghiệp vụ khỏi external library.
/// </summary>
public interface IExcelExportService
{
    /// <summary>
    /// Sinh file Excel bảng điểm cho một lớp học.
    /// </summary>
    byte[] GenerateClassGradesWorkbook(ClassGradeResponse classGrades);
}