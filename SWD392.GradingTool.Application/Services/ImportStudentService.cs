using AutoMapper;
using Microsoft.AspNetCore.Http;
using SWD392.GradingTool.Application.DTOs;
using SWD392.GradingTool.Application.Exceptions;
using SWD392.GradingTool.Application.Interfaces;
using SWD392.GradingTool.Domain.Entities;

namespace SWD392.GradingTool.Application.Services;

public class ImportStudentService : IImportStudentService
{
    private readonly IClassGroupRepository _classGroupRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IMapper _mapper;

    public ImportStudentService(
        IClassGroupRepository classGroupRepository,
        IStudentRepository studentRepository,
        IMapper mapper)
    {
        _classGroupRepository = classGroupRepository;
        _studentRepository = studentRepository;
        _mapper = mapper;
    }

    public async Task<ImportStudentResponseDto> ImportAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        // ─── 1. Validate file ─────────────────────────────────────────────
        if (file == null || file.Length == 0)
            throw new BadRequestException("File không được null hoặc rỗng.");

        var extension = Path.GetExtension(file.FileName);
        if (!extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
            throw new BadRequestException("File phải có định dạng .txt.");

        // ─── 2. Lấy ClassName từ tên file ────────────────────────────────
        var className = Path.GetFileNameWithoutExtension(file.FileName).Trim();

        // ─── 3. Đọc và parse nội dung file ───────────────────────────────
        var students = new List<Student>();
        var seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        int lineNumber = 0;

        using var reader = new StreamReader(file.OpenReadStream());
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            lineNumber++;

            // Bỏ qua dòng rỗng
            if (string.IsNullOrWhiteSpace(line)) continue;

            // ─── 4. Validate format từng dòng ─────────────────────────
            var parts = line.Split(',', 2);
            if (parts.Length < 2)
                throw new BadRequestException(
                    $"Dòng {lineNumber} sai format. Yêu cầu: 'StudentCode, FullName'. Giá trị: '{line.Trim()}'");

            var studentCode = parts[0].Trim();
            var fullName = parts[1].Trim();

            if (string.IsNullOrEmpty(studentCode) || string.IsNullOrEmpty(fullName))
                throw new BadRequestException(
                    $"Dòng {lineNumber}: StudentCode và FullName không được rỗng.");

            // ─── 5. Kiểm tra duplicate trong file ─────────────────────
            if (!seenCodes.Add(studentCode))
                throw new DuplicateException(
                    $"StudentCode '{studentCode}' bị trùng trong file (dòng {lineNumber}).");

            students.Add(new Student
            {
                StudentCode = studentCode,
                FullName = fullName
            });
        }

        if (students.Count == 0)
            throw new BadRequestException("File không có dữ liệu sinh viên hợp lệ.");

        // ─── 6. Kiểm tra duplicate trong DB ──────────────────────────────
        var allCodes = students.Select(s => s.StudentCode);
        var duplicatesInDb = await _studentRepository.GetDuplicateCodesAsync(allCodes, cancellationToken);
        var duplicateList = duplicatesInDb.ToList();

        if (duplicateList.Count > 0)
            throw new DuplicateException(
                $"Các StudentCode sau đã tồn tại trong hệ thống: {string.Join(", ", duplicateList)}");

        // ─── 7. Tạo ClassGroup và gán Students ───────────────────────────
        var classGroup = new ClassGroup
        {
            ClassName = className,
            Students = students
        };

        // ─── 8. Lưu vào database ─────────────────────────────────────────
        var savedGroup = await _classGroupRepository.AddAsync(classGroup, cancellationToken);

        // ─── 9. Map và trả response ───────────────────────────────────────
        var studentDtos = _mapper.Map<List<StudentDto>>(savedGroup.Students);

        return new ImportStudentResponseDto
        {
            Success = true,
            Message = "Import students successfully",
            Data = new ImportStudentDataDto
            {
                ClassId = savedGroup.Id,
                ClassName = savedGroup.ClassName,
                TotalStudents = studentDtos.Count,
                Students = studentDtos
            }
        };
    }
}
