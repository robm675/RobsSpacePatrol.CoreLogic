namespace RobsSpacePatrol.DTO.Library;

public record DeviceDTO
{
    public DeviceDTO(string name, double damageLevel)
    {
        Name = name;
        DamageLevel = damageLevel;
    }

    public string Name { get; set; }
    public double DamageLevel { get; set; }

}