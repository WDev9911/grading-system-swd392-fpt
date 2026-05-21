using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SWD392.GradingTool.Application.DTOs;
using SWD392.GradingTool.Application.Interfaces;

namespace SWD392.GradingTool.API.Controllers;

[ApiController]
[Route("api/rubrics")]
public class RubricsController : ControllerBase
{
    private readonly IRubricService _rubricService;
    private readonly IValidator<CreateRubricRequest> _createValidator;

    public RubricsController(
        IRubricService rubricService,
        IValidator<CreateRubricRequest> createValidator)
    {
        _rubricService = rubricService;
        _createValidator = createValidator;
    }

    /// <summary>
    /// Lấy danh sách tiêu chí chấm điểm
    /// </summary>
    /// <returns>Danh sách Rubric</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRubrics(CancellationToken cancellationToken)
    {
        var rubrics = await _rubricService.GetAllAsync(cancellationToken);

        var response = new
        {
            Success = true,
            Message = "Get rubrics successfully",
            Data = rubrics
        };

        return Ok(response);
    }

    /// <summary>
    /// Thêm thủ công một tiêu chí chấm điểm (rubric / grading component).
    /// </summary>
    /// <remarks>
    /// Bổ trợ cho luồng import file rubric — dùng khi teacher muốn thêm lẻ tẻ một tiêu chí
    /// trực tiếp từ form trên UI mà không cần upload lại file.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateRubric([FromBody] CreateRubricRequest request, CancellationToken cancellationToken)
    {
        // 1. Validate bằng FluentValidation
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Detail = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
            });
        }

        // 2. Gọi service xử lý
        var created = await _rubricService.CreateAsync(request, cancellationToken);

        // 3. Trả về 201 Created
        var response = new
        {
            Success = true,
            Message = "Create rubric successfully",
            Data = created
        };

        return CreatedAtAction(nameof(GetRubrics), response);
    }
}