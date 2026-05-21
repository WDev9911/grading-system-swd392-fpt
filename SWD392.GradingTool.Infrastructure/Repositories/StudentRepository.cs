using Microsoft.EntityFrameworkCore;
using SWD392.GradingTool.Application.Interfaces;
using SWD392.GradingTool.Infrastructure.Data;

namespace SWD392.GradingTool.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;

    public StudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<string>> GetDuplicateCodesAsync(
        IEnumerable<string> studentCodes,
        CancellationToken cancellationToken = default)
    {
        var codeList = studentCodes.ToList();

        // Truy vấn DB một lần, trả về các code đã tồn tại
        var existingCodes = await _context.Students
            .Where(s => codeList.Contains(s.StudentCode))
            .Select(s => s.StudentCode)
            .ToListAsync(cancellationToken);

        return existingCodes;
    }

    public async Task<IEnumerable<Domain.Entities.Student>> GetByClassIdAsync(int classId, CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .Where(s => s.ClassGroupId == classId)
            .OrderBy(s => s.StudentCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Student?> GetByIdAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
    }

    public async Task<Domain.Entities.Student?> GetByIdWithClassAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .Include(s => s.ClassGroup)
            .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Student>> SearchAsync(string keyword, CancellationToken cancellationToken = default)
    {
        // EF.Functions.Like dịch ra SQL LIKE, hiệu quả hơn Contains() ở một số case
        var pattern = $"%{keyword}%";

        return await _context.Students
            .Include(s => s.ClassGroup)
            .Where(s => EF.Functions.Like(s.StudentCode, pattern)
                     || EF.Functions.Like(s.FullName, pattern))
            .OrderBy(s => s.StudentCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Student> AddAsync(Domain.Entities.Student student, CancellationToken cancellationToken = default)
    {
        await _context.Students.AddAsync(student, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return student;
    }
}