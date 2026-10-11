using System.ComponentModel.DataAnnotations;

namespace RobsSpacePatrol.DTO;

public class RegistrationRequestDTO
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}