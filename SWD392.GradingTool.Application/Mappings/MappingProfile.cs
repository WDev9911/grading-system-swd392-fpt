using AutoMapper;
using SWD392.GradingTool.Application.DTOs;
using SWD392.GradingTool.Domain.Entities;

namespace SWD392.GradingTool.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Student entity → StudentDto
        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ClassId, opt => opt.MapFrom(src => src.ClassGroupId));

        // Student entity → StudentSearchDto (kèm ClassName)
        CreateMap<Student, StudentSearchDto>()
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ClassId, opt => opt.MapFrom(src => src.ClassGroupId))
            .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.ClassGroup.ClassName));

        // Rubric entity → RubricDto
        CreateMap<Rubric, RubricDto>()
            .ForMember(dest => dest.RubricId, opt => opt.MapFrom(src => src.Id));

        // ClassGroup entity → ClassDto
        CreateMap<ClassGroup, ClassDto>()
            .ForMember(dest => dest.ClassId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TotalStudents, opt => opt.MapFrom(src => src.Students.Count));

        // Grade entity → GradeItemResponse
        CreateMap<Grade, GradeItemResponse>()
            .ForMember(dest => dest.RubricName, opt => opt.MapFrom(src => src.Rubric.RubricName))
            .ForMember(dest => dest.MaxScore, opt => opt.MapFrom(src => src.Rubric.MaxScore));

        // Grade entity → GradeDetailItem
        CreateMap<Grade, GradeDetailItem>()
            .ForMember(dest => dest.RubricName, opt => opt.MapFrom(src => src.Rubric.RubricName))
            .ForMember(dest => dest.MaxScore, opt => opt.MapFrom(src => src.Rubric.MaxScore));
    }
}