using RobsSpacePatrol.DTO.Library;

namespace RobsSpacePatrol.Library.Models;

public class EnemyFired
{
    public EnemyFired(Coord enemyCoord,
                        int starshipNewShieldAmount,
                        bool starshipDestroyed,
                        Device? deviceDamaged,
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

    public Coord EnemyCoord { get; private set; }
    public int StarshipNewShieldAmount { get; private set; }
    public bool StarshipDestroyed { get; private set; }
    public Device? DeviceDamaged { get; private set; }
    public int EnemyFiredAmount { get; private set; }
    public double DeviceDamangedAmount { get; private set; }
    public bool StarshipDocked { get; private set; }

    public EnemyFiredDTO toDTO => Mapper.ToDTO(this);
    public static EnemyFired fromDTO(EnemyFiredDTO enemyFiredDTO) => Mapper.FromDTO(enemyFiredDTO);
}
