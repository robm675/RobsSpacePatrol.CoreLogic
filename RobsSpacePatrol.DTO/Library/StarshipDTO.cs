using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobsSpacePatrol.DTO.Library;
public class StarshipDTO
{
    public int EnergyLevel { get; set; }
    public int ShieldLevel { get; set; }
    public int TorpedosRemaining { get; set; }
    public bool isDocked { get; set; }
    public bool isDestroyed { get; set; }
    public List<DeviceDTO> Devices { get; set; } = [];

    public StarshipDTO()
    {
        foreach (eDevice device in Enum.GetValues(typeof(eDevice)))
            Devices.Add(new DeviceDTO(device.ToString(), 0));
        EnergyLevel = Global.maxStarshipEnergy;
        TorpedosRemaining = Global.maxStarshipTorpedos;
        isDocked = false;
        isDestroyed = false;
        ShieldLevel = 0;
    }
}
