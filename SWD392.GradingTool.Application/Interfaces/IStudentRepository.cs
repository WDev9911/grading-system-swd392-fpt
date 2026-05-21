namespace SWD392.GradingTool.Application.Interfaces;

public interface IStudentRepository
{
    /// <summary>
    /// Kiểm tra xem có bất kỳ StudentCode nào trong danh sách đã tồn tại trong DB chưa.
    /// Trả về danh sách các code bị trùng.
    /// </summary>
    Task<IEnumerable<string>> GetDuplicateCodesAsync(IEnumerable<string> studentCodes, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Student>> GetByClassIdAsync(int classId, CancellationToken cancellationToken = default);
    Task<Domain.Entities.Student?> GetByIdAsync(int studentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy student kèm theo ClassGroup (dùng cho màn hình chi tiết điểm).
    /// </summary>
    Task<Domain.Entities.Student?> GetByIdWithClassAsync(int studentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tìm sinh viên theo keyword khớp một phần với StudentCode hoặc FullName.
    /// Trả về kèm ClassGroup để hiển thị tên lớp.
    /// </summary>
    Task<IEnumerable<Domain.Entities.Student>> SearchAsync(string keyword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lưu 1 sinh viên mới vào database, trả về entity đã có Id.
    /// </summary>
    Task<Domain.Entities.Student> AddAsync(Domain.Entities.Student student, CancellationToken cancellationToken = default);
}