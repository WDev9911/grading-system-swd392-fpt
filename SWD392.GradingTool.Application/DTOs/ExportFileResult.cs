namespace SWD392.GradingTool.Application.DTOs;

/// <summary>
/// Kết quả của một thao tác export — chứa nội dung file và metadata để controller trả về.
/// </summary>
public class ExportFileResult
{
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}