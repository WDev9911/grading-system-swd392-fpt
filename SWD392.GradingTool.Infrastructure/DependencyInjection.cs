using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWD392.GradingTool.Application.Interfaces;
using SWD392.GradingTool.Infrastructure.Data;
using SWD392.GradingTool.Infrastructure.Repositories;
using SWD392.GradingTool.Infrastructure.Services;

namespace SWD392.GradingTool.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ─── DbContext ────────────────────────────────────────────────────
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // ─── Repositories ─────────────────────────────────────────────────
        services.AddScoped<IClassGroupRepository, ClassGroupRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IRubricRepository, RubricRepository>();
        services.AddScoped<IGradeRepository, GradeRepository>();

        // ─── Infrastructure Services ──────────────────────────────────────
        // ExcelExportService không giữ state → đăng ký Singleton để tránh tạo lại liên tục
        services.AddSingleton<IExcelExportService, ExcelExportService>();

        return services;
    }
}