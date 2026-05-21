using Microsoft.AspNetCore.Mvc;
using SWD392.GradingTool.Application.DTOs;
using SWD392.GradingTool.Application.Interfaces;

namespace SWD392.GradingTool.API.Controllers;

[ApiController]
[Route("api/classes")]
public class ClassesController : ControllerBase
{
    private readonly IClassService _classService;
    private readonly IStudentService _studentService;

    public ClassesController(IClassService classService, IStudentService studentService)
    {
        _classService = classService;
        _studentService = studentService;
    }

    /// <summary>
    /// Lấy danh sách tất cả các lớp/group đã được import
    /// </summary>
    /// <returns>Danh sách lớp kèm theo sĩ số và ngày tạo</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClasses(CancellationToken cancellationToken)
    {
        var classes = await _classService.GetAllAsync(cancellationToken);
        
        // Trả về cấu trúc JSON tương đồng với ApiResponse
        var response = new 
        {
            Success = true,
            Message = "Get classes successfully",
            Data = classes
        };

        return Ok(response);
    }

    /// <summary>
    /// Lấy danh sách sinh viên của một lớp
    /// </summary>
    /// <param name="classId">Id của lớp</param>
    /// <returns>Danh sách sinh viên</returns>
    [HttpGet("{classId}/students")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentsByClass(int classId, CancellationToken cancellationToken)
    {
        var students = await _studentService.GetByClassIdAsync(classId, cancellationToken);
        
        var response = new 
        {
            Success = true,
            Message = "Get students successfully",
            Data = students
        };

        return Ok(response);
    }
}
