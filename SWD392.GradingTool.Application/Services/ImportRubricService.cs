using AutoMapper;
using Microsoft.AspNetCore.Http;
using SWD392.GradingTool.Application.DTOs;
using SWD392.GradingTool.Application.Exceptions;
using SWD392.GradingTool.Application.Interfaces;
using SWD392.GradingTool.Domain.Entities;

namespace SWD392.GradingTool.Application.Services;

public class ImportRubricService : IImportRubricService
{
    private readonly IRubricRepository _rubricRepository;
    private readonly IMapper _mapper;

    public ImportRubricService(IRubricRepository rubricRepository, IMapper mapper)
    {
        _rubricRepository = rubricRepository;
        _mapper = mapper;
    }

    public async Task<ImportRubricResponseDto> ImportAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        // ─── 1. Validate file ─────────────────────────────────────────────
        if (file == null || file.Length == 0)
            throw new BadRequestException("File không được null hoặc rỗng.");

        var extension = Path.GetExtension(file.FileName);
        if (!extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
            throw new BadRequestException("File phải có định dạng .txt.");

        // ─── 2. Đọc và parse từng dòng ────────────────────────────────────
        var rubrics = new List<Rubric>();
        var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        int lineNumber = 0;

        using var reader = new StreamReader(file.OpenReadStream());
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            lineNumber++;

            // Bỏ qua dòng rỗng
            if (string.IsNullOrWhiteSpace(line)) continue;

            // ─── 3. Validate format dòng ──────────────────────────────────
            var parts = line.Split('|', 2);
            if (parts.Length < 2)
                throw new BadRequestException(
                    $"Dòng {lineNumber} sai format. Yêu cầu: 'RubricName|MaxScore'. Giá trị: '{line.Trim()}'");

            var rubricName = parts[0].Trim();
            var maxScoreRaw = parts[1].Trim();

            if (string.IsNullOrEmpty(rubricName))
                throw new BadRequestException($"Dòng {lineNumber}: RubricName không được rỗng.");

            // ─── 4. Validate MaxScore là số hợp lệ > 0 ───────────────────
            if (!decimal.TryParse(maxScoreRaw, out var maxScore))
                throw new BadRequestException(
                    $"Dòng {lineNumber}: MaxScore '{maxScoreRaw}' không phải số hợp lệ.");

            if (maxScore <= 0)
                throw new BadRequestException(
                    $"Dòng {lineNumber}: MaxScore phải lớn hơn 0. Giá trị hiện tại: {maxScore}");

            // ─── 5. Kiểm tra duplicate trong file ─────────────────────────
            if (!seenNames.Add(rubricName))
                throw new DuplicateException(
                    $"RubricName '{rubricName}' bị trùng trong file (dòng {lineNumber}).");

            rubrics.Add(new Rubric
            {
                RubricName = rubricName,
                MaxScore = maxScore,
                CreatedDate = DateTime.UtcNow
            });
        }

        if (rubrics.Count == 0)
            throw new BadRequestException("File không có dữ liệu rubric hợp lệ.");

        // ─── 6. Kiểm tra duplicate trong DB ──────────────────────────────
        var allNames = rubrics.Select(r => r.RubricName);
        var duplicatesInDb = await _rubricRepository.GetDuplicateNamesAsync(allNames, cancellationToken);
        var duplicateList = duplicatesInDb.ToList();

        if (duplicateList.Count > 0)
            throw new DuplicateException(
                $"Các RubricName sau đã tồn tại trong hệ thống: {string.Join(", ", duplicateList)}");

        // ─── 7. Lưu vào database ─────────────────────────────────────────
        var savedRubrics = await _rubricRepository.AddRangeAsync(rubrics, cancellationToken);

        // ─── 8. Map và trả response ───────────────────────────────────────
        var rubricDtos = _mapper.Map<List<RubricDto>>(savedRubrics);

        return new ImportRubricResponseDto
        {
            Success = true,
            Message = "Import rubrics successfully",
            Data = new ImportRubricDataDto
            {
                TotalRubrics = rubricDtos.Count,
                Rubrics = rubricDtos
            }
        };
    }
}
