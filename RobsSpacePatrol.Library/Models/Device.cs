using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobsSpacePatrol.DTO.Library;

namespace RobsSpacePatrol.Library.Models;
public record Device
{
    public Device(string name, double damageLevel)
    {
        Name = name;
        DamageLevel = damageLevel;
    }

    public string Name { get; set; }
    public double DamageLevel { get; set; }

    public DeviceDTO toDTO => new DeviceDTO(Name, DamageLevel);
    public static Device fromDTO(DeviceDTO deviceDTO) => new Device(deviceDTO.Name, deviceDTO.DamageLevel);


}
