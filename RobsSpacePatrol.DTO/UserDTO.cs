using System.ComponentModel.DataAnnotations;

namespace RobsSpacePatrol.DTO;

public class UserDTO
{
    [Key]
    public string Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Role { get; set; } = default!;
}
