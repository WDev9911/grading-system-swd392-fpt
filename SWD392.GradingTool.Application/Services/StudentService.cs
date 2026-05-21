using AutoMapper;
using SWD392.GradingTool.Application.DTOs;
using SWD392.GradingTool.Application.Exceptions;
using SWD392.GradingTool.Application.Interfaces;
using SWD392.GradingTool.Domain.Entities;

namespace SWD392.GradingTool.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IClassGroupRepository _classGroupRepository;
    private readonly IMapper _mapper;

    public StudentService(
        IStudentRepository studentRepository,
        IClassGroupRepository classGroupRepository,
        IMapper mapper)
    {
        _studentRepository = studentRepository;
        _classGroupRepository = classGroupRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<StudentDto>> GetByClassIdAsync(int classId, CancellationToken cancellationToken = default)
    {
        // 1. Kiểm tra class có tồn tại hay không
        var classGroup = await _classGroupRepository.GetByIdAsync(classId, cancellationToken);
        if (classGroup == null)
        {
            throw new NotFoundException($"Không tìm thấy lớp học với Id = {classId}");
        }

        // 2. Lấy danh sách sinh viên
        var students = await _studentRepository.GetByClassIdAsync(classId, cancellationToken);

        // 3. Mapping
        return _mapper.Map<IEnumerable<StudentDto>>(students);
    }

    public async Task<IEnumerable<StudentSearchDto>> SearchAsync(string keyword, CancellationToken cancellationToken = default)
    {
        // 1. Validate keyword
        if (string.IsNullOrWhiteSpace(keyword))
        {
            throw new BadRequestException("Keyword không được để trống.");
        }

        // 2. Search ở repository (trim để tránh khoảng trắng thừa từ FE)
        var students = await _studentRepository.SearchAsync(keyword.Trim(), cancellationToken);

        // 3. Mapping (trả về list rỗng nếu không tìm thấy, không throw NotFound)
        return _mapper.Map<IEnumerable<StudentSearchDto>>(students);
    }

    public async Task<StudentSearchDto> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Trim input
        var studentCode = request.StudentCode.Trim();
        var fullName = request.FullName.Trim();

        // 2. Validate ClassGroup tồn tại
        var classGroup = await _classGroupRepository.GetByIdAsync(request.ClassGroupId, cancellationToken);
        if (classGroup == null)
        {
            throw new NotFoundException($"Không tìm thấy lớp học với Id = {request.ClassGroupId}");
        }

        // 3. Check duplicate StudentCode trên toàn DB (tái dụng method có sẵn của Import flow)
        // Note: StudentCode là unique toàn hệ thống, không scoped theo class
        var duplicates = await _studentRepository.GetDuplicateCodesAsync(new[] { studentCode }, cancellationToken);
        if (duplicates.Any())
        {
            throw new DuplicateException($"StudentCode '{studentCode}' đã tồn tại trong hệ thống.");
        }

        // 4. Tạo entity và lưu DB
        var student = new Student
        {
            StudentCode = studentCode,
            FullName = fullName,
            ClassGroupId = classGroup.Id,
            ClassGroup = classGroup    // gán sẵn nav property để mapping kèm ClassName mà không cần re-query
        };

        var saved = await _studentRepository.AddAsync(student, cancellationToken);

        // 5. Mapping trả về (kèm ClassName)
        return _mapper.Map<StudentSearchDto>(saved);
    }
}