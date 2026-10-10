using static RobsSpacePatrol.DTO.SD;

namespace RobsSpacePatrol.DTO;

public class ApiRequest
{
    public ApiType ApiType { get; set; } = ApiType.GET;
    public string? Url { get; set; }
    public object? Data { get; set; }
    public string? Token { get; set; }
}
