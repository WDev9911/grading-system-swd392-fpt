using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SWD392.GradingTool.Application.Interfaces;
using SWD392.GradingTool.Application.Mappings;
using SWD392.GradingTool.Application.Services;

namespace SWD392.GradingTool.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper – đăng ký thủ công vì AutoMapper 13 không cần Extensions package
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        services.AddSingleton<IMapper>(mapperConfig.CreateMapper());

        // Services
        services.AddScoped<IImportStudentService, ImportStudentService>();
        services.AddScoped<IImportRubricService, ImportRubricService>();
        services.AddScoped<IClassService, ClassService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IRubricService, RubricService>();
        services.AddScoped<IGradeService, GradeService>();

        // Validators
        services.AddValidatorsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());

        return services;
    }
}
