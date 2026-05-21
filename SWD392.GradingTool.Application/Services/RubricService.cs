using AutoMapper;
using SWD392.GradingTool.Application.DTOs;
using SWD392.GradingTool.Application.Exceptions;
using SWD392.GradingTool.Application.Interfaces;
using SWD392.GradingTool.Domain.Entities;

namespace SWD392.GradingTool.Application.Services;

public class RubricService : IRubricService
{
    private readonly IRubricRepository _rubricRepository;
    private readonly IMapper _mapper;

    public RubricService(IRubricRepository rubricRepository, IMapper mapper)
    {
        _rubricRepository = rubricRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RubricDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rubrics = await _rubricRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<RubricDto>>(rubrics);
    }

    public async Task<RubricDto> CreateAsync(CreateRubricRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Trim input để tránh tên rubric có khoảng trắng dư từ FE
        var rubricName = request.RubricName.Trim();

        // 2. Check duplicate trong DB (tái dụng method có sẵn của Import flow)
        var duplicates = await _rubricRepository.GetDuplicateNamesAsync(new[] { rubricName }, cancellationToken);
        if (duplicates.Any())
        {
            throw new DuplicateException($"RubricName '{rubricName}' đã tồn tại trong hệ thống.");
        }

        // 3. Tạo entity và lưu DB
        var rubric = new Rubric
        {
            RubricName = rubricName,
            MaxScore = request.MaxScore,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            CreatedDate = DateTime.UtcNow
        };

        var saved = await _rubricRepository.AddAsync(rubric, cancellationToken);

        // 4. Mapping trả về
        return _mapper.Map<RubricDto>(saved);
    }
}