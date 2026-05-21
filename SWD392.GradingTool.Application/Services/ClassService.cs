using AutoMapper;
using SWD392.GradingTool.Application.DTOs;
using SWD392.GradingTool.Application.Interfaces;

namespace SWD392.GradingTool.Application.Services;

public class ClassService : IClassService
{
    private readonly IClassGroupRepository _classGroupRepository;
    private readonly IMapper _mapper;

    public ClassService(IClassGroupRepository classGroupRepository, IMapper mapper)
    {
        _classGroupRepository = classGroupRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ClassDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var classes = await _classGroupRepository.GetAllWithStudentsAsync(cancellationToken);
        
        // AutoMapper will handle the mapping, including TotalStudents calculation if configured correctly
        // Alternatively, we can map it here or configure the map profile. We'll use AutoMapper profile.
        return _mapper.Map<IEnumerable<ClassDto>>(classes);
    }
}
