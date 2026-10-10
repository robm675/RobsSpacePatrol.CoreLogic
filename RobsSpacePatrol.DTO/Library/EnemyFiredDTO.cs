using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobsSpacePatrol.DTO.Library;
public class EnemyFiredDTO
{
    public EnemyFiredDTO(CoordDTO enemyCoord,
                    int starshipNewShieldAmount,
                    bool starshipDestroyed,
                    DeviceDTO? deviceDamaged,
                    double deviceDamangedAmount,
                    int enemyFiredAmount,
                    bool starshipDocked)
    {
        EnemyCoord = enemyCoord;
        StarshipNewShieldAmount = starshipNewShieldAmount;
        StarshipDestroyed = starshipDestroyed;
        DeviceDamaged = deviceDamaged;
        DeviceDamangedAmount = deviceDamangedAmount;
        EnemyFiredAmount = enemyFiredAmount;
        StarshipDocked = starshipDocked;
    }

    public CoordDTO EnemyCoord { get; private set; }
    public int StarshipNewShieldAmount { get; private set; }
    public bool StarshipDestroyed { get; private set; }
    public DeviceDTO? DeviceDamaged { get; private set; }
    public int EnemyFiredAmount { get; private set; }
    public double DeviceDamangedAmount { get; private set; }
    public bool StarshipDocked { get; private set; }
}
