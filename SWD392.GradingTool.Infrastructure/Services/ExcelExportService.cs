using ClosedXML.Excel;
using SWD392.GradingTool.Application.DTOs;
using SWD392.GradingTool.Application.Interfaces;

namespace SWD392.GradingTool.Infrastructure.Services;

/// <summary>
/// Implementation của IExcelExportService dùng ClosedXML.
/// Đặt ở tầng Infrastructure vì phụ thuộc external library cụ thể.
/// </summary>
public class ExcelExportService : IExcelExportService
{
    public byte[] GenerateClassGradesWorkbook(ClassGradeResponse classGrades)
    {
        using var workbook = new XLWorkbook();
        // Tên sheet trong Excel giới hạn 31 ký tự
        var sheetName = TruncateSheetName(classGrades.ClassName);
        var ws = workbook.Worksheets.Add(string.IsNullOrWhiteSpace(sheetName) ? "Grades" : sheetName);

        // ─── 1. Canonicalize danh sách rubric (union từ tất cả students, sort theo RubricId) ─
        // Lý do: mỗi student có thể có thứ tự rubric khác nhau, hoặc có student thiếu rubric chưa chấm.
        // Ta cần 1 thứ tự cột thống nhất cho toàn bảng.
        var rubricColumns = classGrades.Students
            .SelectMany(s => s.Grades)
            .GroupBy(g => g.RubricId)
            .Select(g => new
            {
                RubricId = g.Key,
                RubricName = g.First().RubricName,
                MaxScore = g.First().MaxScore
            })
            .OrderBy(r => r.RubricId)
            .ToList();

        // ─── 2. Vẽ header row ──────────────────────────────────────────────
        // Layout: STT | StudentCode | FullName | <Rubric1 (Max)> | <Rubric2 (Max)> | ... | Total | Comment
        int col = 1;
        ws.Cell(1, col++).Value = "STT";
        ws.Cell(1, col++).Value = "StudentCode";
        ws.Cell(1, col++).Value = "FullName";

        int firstRubricCol = col;
        foreach (var r in rubricColumns)
        {
            ws.Cell(1, col++).Value = $"{r.RubricName} ({r.MaxScore})";
        }
        int lastRubricCol = col - 1;

        int totalCol = col++;
        ws.Cell(1, totalCol).Value = "Total";

        int commentCol = col;
        ws.Cell(1, commentCol).Value = "Comment";

        // ─── 3. Styling header ────────────────────────────────────────────
        var headerRange = ws.Range(1, 1, 1, commentCol);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        // ─── 4. Vẽ data rows ──────────────────────────────────────────────
        int row = 2;
        int stt = 1;
        foreach (var student in classGrades.Students)
        {
            // Tra cứu nhanh grade của student theo RubricId
            var gradeByRubric = student.Grades.ToDictionary(g => g.RubricId);

            col = 1;
            ws.Cell(row, col++).Value = stt++;
            ws.Cell(row, col++).Value = student.StudentCode;
            ws.Cell(row, col++).Value = student.FullName;

            // Điểm từng rubric — để trống nếu chưa có grade
            foreach (var r in rubricColumns)
            {
                if (gradeByRubric.TryGetValue(r.RubricId, out var grade))
                {
                    ws.Cell(row, col).Value = grade.Score;
                }
                col++;
            }

            // Total = công thức SUM của các cột rubric → teacher đổi điểm thì tự tính lại
            // Range vd: "D2:G2" khi rubric ở cột 4-7
            var firstRubricCellRef = ws.Cell(row, firstRubricCol).Address.ToString();
            var lastRubricCellRef = ws.Cell(row, lastRubricCol).Address.ToString();
            ws.Cell(row, totalCol).FormulaA1 = $"=SUM({firstRubricCellRef}:{lastRubricCellRef})";

            // Comment — gộp comment của tất cả rubric vào 1 cell, format "ERD: ... | REST API: ..."
            var combinedComment = string.Join(" | ",
                student.Grades
                    .Where(g => !string.IsNullOrWhiteSpace(g.Comment))
                    .OrderBy(g => g.RubricId)
                    .Select(g => $"{g.RubricName}: {g.Comment}"));
            ws.Cell(row, commentCol).Value = combinedComment;

            row++;
        }

        // ─── 5. Styling data rows ─────────────────────────────────────────
        if (classGrades.Students.Count > 0)
        {
            var dataRange = ws.Range(2, 1, row - 1, commentCol);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Cột STT, scores, Total: căn giữa
            ws.Range(2, 1, row - 1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(2, firstRubricCol, row - 1, totalCol).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Cột Total: bold (highlight cho dễ nhìn)
            ws.Range(2, totalCol, row - 1, totalCol).Style.Font.Bold = true;

            // Cột Comment: wrap text để text dài không tràn
            ws.Range(2, commentCol, row - 1, commentCol).Style.Alignment.WrapText = true;
        }

        // ─── 6. Auto-fit columns + freeze header row ──────────────────────
        ws.Columns().AdjustToContents();
        // Giới hạn độ rộng cột Comment để không quá dài
        if (ws.Column(commentCol).Width > 60)
        {
            ws.Column(commentCol).Width = 60;
        }
        ws.SheetView.FreezeRows(1);

        // ─── 7. Xuất ra byte[] ────────────────────────────────────────────
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static string TruncateSheetName(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        // Một số ký tự bị Excel cấm trong tên sheet
        var invalidChars = new[] { '\\', '/', '?', '*', '[', ']', ':' };
        var cleaned = new string(name.Where(c => !invalidChars.Contains(c)).ToArray());

        return cleaned.Length > 31 ? cleaned[..31] : cleaned;
    }
}