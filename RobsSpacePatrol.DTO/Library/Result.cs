
namespace RobsSpacePatrol.DTO.Library;

public class Result
{
    public class SRS
    {
        public SRS(List<SectorDTO> sectors)
        {
            Sectors = sectors;
        }

        public List<SectorDTO> Sectors { get; set; } = new();
    }
    public class LRS
    {
        public List<QuadrantDTO> Quadrants { get; set; } = new();

        public LRS(List<QuadrantDTO> quadrants)
        {
            Quadrants = quadrants;
        }
    }
    public class COM_REC
    {
        public COM_REC(List<QuadrantDTO> quadrants)
        {
            Quadrants = quadrants;
        }

        public List<QuadrantDTO> Quadrants { get; set; } = new();
    }
    public class COM_STA
    {
        public COM_STA(double missionTime, CoordDTO quadrantCoord, CoordDTO sectorCoord, List<DeviceDTO> damagedDevices, int enemiesRemaining, int starBaseRemaining, int energyRemaining, int shieldLevel, int torpedosRemaining, bool docked, double missionTimeDeadLine)
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
        public List<DeviceDTO> DamagedDevices { get; set; } = new();
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
        public COM_TOR(List<DirectionToEnemyDTO> directionToEnemies)
        {
            DirectionToEnemies = directionToEnemies;
        }

        public List<DirectionToEnemyDTO> DirectionToEnemies { get; set; } = new();
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
        public DAM(List<DeviceDTO> devices)
        {
            Devices = devices;
        }

        public List<DeviceDTO> Devices { get; set; } = new();
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
        public TOR(ObjectHitReportDTO objectHitReport)
        {
            ObjectHitReport = objectHitReport;
        }

        public ObjectHitReportDTO ObjectHitReport { get; set; }
    }
    public class LAS
    {
        public LAS(List<ObjectHitReportDTO> objectHitReport)
        {
            ObjectHitReport = objectHitReport;
        }

        public List<ObjectHitReportDTO> ObjectHitReport { get; set; }
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
        public MaintResult(double newMissionTime, List<DeviceDTO>? devicesRepairStatusChanged, IList<EnemyFiredDTO>? enemiesFired, IList<EnemyMovedDTO>? enemiesMoved, DockingStatusArgDTO? dockingStatus, double? starbaseRepairsMissionTimeUnits, eGameStatus gameStatus, bool AnyEnemiesInQuadrant, int StarshipShieldLevel)
        {
            this.newMissionTime = newMissionTime;
            this.DevicesRepairStatusChanged = devicesRepairStatusChanged;
            this.EnemiesFired = enemiesFired;
            this.EnemiesMoved = enemiesMoved;
            this.DockingStatus = dockingStatus;
            this.StarbaseRepairsMissionTimeUnits = starbaseRepairsMissionTimeUnits;
            this.GameStatus = gameStatus;
            this.AnyEnemiesInQuadrant = AnyEnemiesInQuadrant;
            this.StarshipShieldLevel = StarshipShieldLevel;
        }

        public double newMissionTime { get; set; }
        public List<DeviceDTO>? DevicesRepairStatusChanged { get; set; }
        public IList<EnemyFiredDTO>? EnemiesFired { get; set; }
        public IList<EnemyMovedDTO>? EnemiesMoved { get; set; }
        public DockingStatusArgDTO? DockingStatus { get; set; }
        public double? StarbaseRepairsMissionTimeUnits { get; set; }
        public eGameStatus GameStatus { get; set; }
        public bool AnyEnemiesInQuadrant { get; set; }
        public int StarshipShieldLevel { get; set; }
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
