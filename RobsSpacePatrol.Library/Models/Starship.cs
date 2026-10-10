using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobsSpacePatrol.DTO.Library;

namespace RobsSpacePatrol.Library.Models;

public record Starship
{
    public int EnergyLevel { get; set; }
    public int ShieldLevel { get; set; }
    public int TorpedosRemaining { get; set; }
    public bool isDocked { get; set; }
    public bool isDestroyed { get; set; }
    public List<Device> Devices { get; set; } = [];
    public Starship()
    {
        foreach (eDevice device in Enum.GetValues(typeof(eDevice)))
            Devices.Add(new Device(device.ToString(),0));
        EnergyLevel = Global.maxStarshipEnergy;
        TorpedosRemaining = Global.maxStarshipTorpedos;
        isDocked = false;
        isDestroyed = false;
        ShieldLevel = 0;
    }
    public string getDevice(eDevice device)
    {
        return device.ToString();
    }
    public eDevice? getDevice(string deviceName)
    {
        try
        {
            return (eDevice)Enum.Parse(typeof(eDevice), deviceName);
        }
        catch (Exception)
        {
            return null;
        }
    }
    public void setDeviceDamage(eDevice device, double damageLevel)
    {
        var targetDevice = Devices.First(z => z.Name == getDevice(device));
        if (targetDevice == null) return;
        targetDevice.DamageLevel = damageLevel;
    }
    public double getDeviceDamage(eDevice device)
    {
        return Devices.First(z => z.Name == getDevice(device)).DamageLevel;
    }

    public List<Device> GetDamagedDevices => Devices.Where(z => z.DamageLevel < 0).ToList();

    public StarshipDTO toDTO => Mapper.ToDTO(this);
    public static Starship fromDTO(StarshipDTO starshipDTO) => Mapper.FromDTO(starshipDTO);

    public List<DeviceDTO> toDeviceDTO => Mapper.ToDTO(this.Devices);
    public static List<Device> fromDeviceDTO(List<DeviceDTO> deviceDTOs) =>
        Mapper.FromDTO(deviceDTOs);

}
