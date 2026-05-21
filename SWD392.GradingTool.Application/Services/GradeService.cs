using AutoMapper;
using SWD392.GradingTool.Application.DTOs;
using SWD392.GradingTool.Application.Exceptions;
using SWD392.GradingTool.Application.Interfaces;
using SWD392.GradingTool.Domain.Entities;

namespace SWD392.GradingTool.Application.Services;

public class GradeService : IGradeService
{
    private readonly IGradeRepository _gradeRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IRubricRepository _rubricRepository;
    private readonly IClassGroupRepository _classGroupRepository;
    private readonly IExcelExportService _excelExportService;
    private readonly IMapper _mapper;

    public GradeService(
        IGradeRepository gradeRepository,
        IStudentRepository studentRepository,
        IRubricRepository rubricRepository,
        IClassGroupRepository classGroupRepository,
        IExcelExportService excelExportService,
        IMapper mapper)
    {
        _gradeRepository = gradeRepository;
        _studentRepository = studentRepository;
        _rubricRepository = rubricRepository;
        _classGroupRepository = classGroupRepository;
        _excelExportService = excelExportService;
        _mapper = mapper;
    }

    public async Task<GradeResponse> GradeStudentAsync(GradeStudentRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Validate student
        var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
        if (student == null)
        {
            throw new NotFoundException($"Không tìm thấy sinh viên với Id = {request.StudentId}");
        }

        // 2. Lấy thông tin rubrics để validate
        var rubricIds = request.Grades.Select(g => g.RubricId).Distinct().ToList();
        var rubrics = await _rubricRepository.GetByIdsAsync(rubricIds, cancellationToken);

        var rubricDictionary = rubrics.ToDictionary(r => r.Id);

        foreach (var gradeReq in request.Grades)
        {
            // Validate rubric exists
            if (!rubricDictionary.TryGetValue(gradeReq.RubricId, out var rubric))
            {
                throw new BadRequestException($"RubricId = {gradeReq.RubricId} không tồn tại.");
            }

            // Validate score range
            if (gradeReq.Score < 0 || gradeReq.Score > rubric.MaxScore)
            {
                throw new BadRequestException($"Điểm cho rubric '{rubric.RubricName}' phải từ 0 đến {rubric.MaxScore}.");
            }
        }

        // 3. Upsert grades
        foreach (var gradeReq in request.Grades)
        {
            var existingGrade = await _gradeRepository.GetByStudentAndRubricAsync(request.StudentId, gradeReq.RubricId, cancellationToken);

            if (existingGrade == null)
            {
                var newGrade = new Grade
                {
                    StudentId = request.StudentId,
                    RubricId = gradeReq.RubricId,
                    Score = gradeReq.Score,
                    Comment = gradeReq.Comment
                };
                await _gradeRepository.AddAsync(newGrade, cancellationToken);
            }
            else
            {
                existingGrade.Score = gradeReq.Score;
                existingGrade.Comment = gradeReq.Comment;
                await _gradeRepository.UpdateAsync(existingGrade, cancellationToken);
            }
        }

        // 4. Lấy lại danh sách điểm để trả về
        var finalGrades = await _gradeRepository.GetGradesByStudentAsync(request.StudentId, cancellationToken);

        // 5. Tính tổng điểm
        var totalScore = finalGrades.Sum(g => g.Score);

        // 6. Mapping response
        var response = new GradeResponse
        {
            StudentId = student.Id,
            StudentCode = student.StudentCode,
            FullName = student.FullName,
            TotalScore = totalScore,
            Grades = _mapper.Map<List<GradeItemResponse>>(finalGrades)
        };

        return response;
    }

    public async Task<ClassGradeResponse> GetGradesByClassAsync(int classId, CancellationToken cancellationToken = default)
    {
        // 1. Validate class
        var classGroup = await _classGroupRepository.GetByIdAsync(classId, cancellationToken);
        if (classGroup == null)
        {
            throw new NotFoundException($"Không tìm thấy lớp học với Id = {classId}");
        }

        // 2. Get students by class
        var students = await _studentRepository.GetByClassIdAsync(classId, cancellationToken);

        // 3. Get grades of each student in class
        var grades = await _gradeRepository.GetGradesByClassAsync(classId, cancellationToken);

        // 4. Group grades by student
        var gradesByStudent = grades.GroupBy(g => g.StudentId).ToDictionary(g => g.Key, g => g.ToList());

        // 5. Construct response
        var classGradeResponse = new ClassGradeResponse
        {
            ClassId = classGroup.Id,
            ClassName = classGroup.ClassName,
            Students = new List<StudentGradeResponse>()
        };

        foreach (var student in students)
        {
            var studentGrades = gradesByStudent.GetValueOrDefault(student.Id, new List<Grade>());
            var totalScore = studentGrades.Sum(g => g.Score);

            classGradeResponse.Students.Add(new StudentGradeResponse
            {
                StudentId = student.Id,
                StudentCode = student.StudentCode,
                FullName = student.FullName,
                TotalScore = totalScore,
                Grades = _mapper.Map<List<GradeItemResponse>>(studentGrades)
            });
        }

        return classGradeResponse;
    }

    public async Task<StudentGradeDetailResponse> GetStudentGradeDetailsAsync(int studentId, CancellationToken cancellationToken = default)
    {
        // 1. Validate student tồn tại (kèm thông tin ClassGroup để lấy ClassName)
        var student = await _studentRepository.GetByIdWithClassAsync(studentId, cancellationToken);
        if (student == null)
        {
            throw new NotFoundException($"Không tìm thấy sinh viên với Id = {studentId}");
        }

        // 2. Lấy toàn bộ grade của student (đã Include Rubric trong repository)
        var grades = await _gradeRepository.GetGradesByStudentAsync(studentId, cancellationToken);

        // 3. Tính total score
        var totalScore = grades.Sum(g => g.Score);

        // 4. Map sang DTO trả về
        var response = new StudentGradeDetailResponse
        {
            StudentId = student.Id,
            StudentCode = student.StudentCode,
            FullName = student.FullName,
            ClassName = student.ClassGroup?.ClassName ?? string.Empty,
            TotalScore = totalScore,
            Grades = _mapper.Map<List<GradeDetailItem>>(grades)
        };

        return response;
    }

    public async Task<ExportFileResult> ExportClassGradesAsync(int classId, CancellationToken cancellationToken = default)
    {
        // 1. Tái dụng GetGradesByClassAsync — đã có sẵn logic validate class + lấy students + grades
        var classGrades = await GetGradesByClassAsync(classId, cancellationToken);

        // 2. Generate file Excel qua infrastructure service
        var bytes = _excelExportService.GenerateClassGradesWorkbook(classGrades);

        // 3. Đóng gói kết quả với tên file + content type chuẩn
        var safeClassName = SanitizeFileName(classGrades.ClassName);
        var fileName = $"{safeClassName}_Grades_{DateTime.UtcNow:yyyyMMdd}.xlsx";

        return new ExportFileResult
        {
            Content = bytes,
            FileName = fileName,
            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        };
    }

    /// <summary>
    /// Loại bỏ ký tự không hợp lệ trong tên file (Windows/Linux/MacOS).
    /// </summary>
    private static string SanitizeFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "Class";

        var invalid = Path.GetInvalidFileNameChars();
        var cleaned = new string(name.Where(c => !invalid.Contains(c)).ToArray()).Trim();

        return string.IsNullOrWhiteSpace(cleaned) ? "Class" : cleaned;
    }
}