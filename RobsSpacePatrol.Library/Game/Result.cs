
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library.eNums;
using RobsSpacePatrol.Library.Models;

namespace RobsSpacePatrol.Library.Game;

public class Result
{
    public class SRS
    {
        public SRS(List<Sector> sectors)
        {
            Sectors = sectors;
        }

        public List<Sector> Sectors { get; set; } = new();
    }
    public class LRS
    {
        public List<Quadrant> Quadrants { get; set; } = new();

        public LRS(List<Quadrant> quadrants)
        {
            Quadrants = quadrants;
        }
    }
    public class COM_REC
    {
        public COM_REC(List<Quadrant> quadrants)
        {
            Quadrants = quadrants;
        }

        public List<Quadrant> Quadrants { get; set; } = new();
    }
    public class COM_STA
    {
        public COM_STA(double missionTime, CoordDTO quadrantCoord, CoordDTO sectorCoord, List<Device> damagedDevices, int enemiesRemaining, int starBaseRemaining, int energyRemaining, int shieldLevel, int torpedosRemaining, bool docked, double missionTimeDeadLine)
        {
            MissionTime = missionTime;
            QuadrantCoord = quadrantCoord;
            SectorCoord = sectorCoord;
            DamagedDevices = damagedDevices;
            EnemiesRemaining = enemiesRemaining;
            StarBaseRemaining = starBaseRemaining;
            EnergyRemaining = energyRemaining;
            ShieldLevel = shieldLevel;
            TorpedosRemaining = torpedosRemaining;
            Docked = docked;
            MissionTimeDeadLine = missionTimeDeadLine;
        }

        public double MissionTime { get; set; } = 0;
        public CoordDTO QuadrantCoord { get; set; }
        public CoordDTO SectorCoord { get; set; }
        public List<Device> DamagedDevices { get; set; } = new();
        public int EnemiesRemaining { get; set; } = 0;
        public int StarBaseRemaining { get; set; } = 0;
        public int EnergyRemaining { get; set; } = 0;
        public int ShieldLevel { get; set; } = 0;
        public int TorpedosRemaining { get; set; } = 0;
        public bool Docked { get; set; } = false;
        public double MissionTimeDeadLine { get; set; } = 0;
    }
    public class COM_STB
    {
        public COM_STB(double direction, double distance, CoordDTO starBaseCoord)
        {
            Direction = direction;
            Distance = distance;
            StarBaseCoord = starBaseCoord;
        }

        public double Direction { get; set; }
        public double Distance { get; set; }
        public CoordDTO StarBaseCoord { get; set; }
    }
    public class COM_TOR
    {
        public COM_TOR(List<DirectionToEnemy> directionToEnemies)
        {
            DirectionToEnemies = directionToEnemies;
        }

        public List<DirectionToEnemy> DirectionToEnemies { get; set; } = new();
    }
    public class COM_NAV
    {
        public COM_NAV(double direction, double distance)
        {
            Direction = direction;
            Distance = distance;
        }

        public double Direction { get; set; }
        public double Distance { get; set; }
    }
    public class DAM
    {
        public DAM(List<Device> devices)
        {
            Devices = devices;
        }

        public List<Device> Devices { get; set; } = new();
    }
    public class SHE
    {
        public SHE(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; set; } = string.Empty;
    }
    public class TOR
    {
        public TOR(ObjectHitReport objectHitReport)
        {
            ObjectHitReport = objectHitReport;
        }

        public ObjectHitReport ObjectHitReport { get; set; }
    }
    public class LAS
    {
        public LAS(List<ObjectHitReport> objectHitReport)
        {
            ObjectHitReport = objectHitReport;
        }

        public List<ObjectHitReport> ObjectHitReport { get; set; }
    }
    public class NAV
    {
        public NAV(eNavMessage navMessage, double distanceTraveled)
        {
            NavMessage = navMessage;
            DistanceTraveled = distanceTraveled;
        }

        public eNavMessage NavMessage { get; set; }
        public double DistanceTraveled { get; set; }
    }
    public class MaintResult
    {
        public MaintResult(double newMissionTime, List<Device>? devicesRepairStatusChanged, IList<EnemyFired>? enemiesFired, IList<EnemyMoved>? enemiesMoved, DockingStatusArg? dockingStatus, StarbaseRepairs? starbaseRepairsAvailable, eGameStatus gameStatus)
        {
            this.newMissionTime = newMissionTime;
            DevicesRepairStatusChanged = devicesRepairStatusChanged;
            EnemiesFired = enemiesFired;
            EnemiesMoved = enemiesMoved;
            DockingStatus = dockingStatus;
            StarbaseRepairsAvailable = starbaseRepairsAvailable;
            GameStatus = gameStatus;
        }

        public double newMissionTime { get; set; }
        public List<Device>? DevicesRepairStatusChanged { get; set; }
        public IList<EnemyFired>? EnemiesFired { get; set; }
        public IList<EnemyMoved>? EnemiesMoved { get; set; }
        public DockingStatusArg? DockingStatus { get; set; }
        public StarbaseRepairs? StarbaseRepairsAvailable { get; set; }
        public eGameStatus GameStatus { get; set; }
    }



    public eCommands Command { get; set; } = eCommands.NONE;
    private object? _data = null;
    public object? Data
    {
        get { return _data; }
        set
        {
            _data = value;
        }
    }
    public eCommandResult eCommandResult { get; set; } = eCommandResult.None;
    public T getResults<T>() where T : class
    {
        T? returnData = _data as T;

        return returnData!;
    }
}
