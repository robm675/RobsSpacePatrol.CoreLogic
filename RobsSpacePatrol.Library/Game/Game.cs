using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Factories;
using RobsSpacePatrol.Library.eNums;
using RobsSpacePatrol.Library.Factories.Interfaces;
using RobsSpacePatrol.Library.Models;

namespace RobsSpacePatrol.Library.Game;


//Y is up and down
//X is left and right

public class Game : IGame
{
    private IGalaxy _galaxy;
    public IRandomFactory RandomFactory => _randomFactory;
    public ICurrentQuadrantFactory CurrentQuadrantFactory => _currentQuadrantFactory;
    private readonly IRandomFactory _randomFactory;
    private readonly ICurrentQuadrantFactory _currentQuadrantFactory;
    public virtual IGalaxy Galaxy { get => _galaxy; }

    public Game(IGalaxy galaxy, IRandomFactory randomFactory, ICurrentQuadrantFactory currentQuadrantFactory)
    {
        _galaxy = galaxy;
        _randomFactory = randomFactory;
        _currentQuadrantFactory = currentQuadrantFactory;
    }

    public Result SRS()
    {
        if (Galaxy.Starship.getDeviceDamage(eDevice.SRS) < 0)
            return new Result
            {
                Data = null,
                Command = eCommands.SRS,
                eCommandResult = eCommandResult.Damaged
            };
        return new Result
        {
            Data = new Result.SRS(Galaxy.CurrentQuadrant.toSectorDTO),
            Command = eCommands.SRS,
            eCommandResult = eCommandResult.OK
        };
    }

    public Result LRS()
    {
        if (Galaxy.Starship.getDeviceDamage(eDevice.LRS) < 0)
        {
            return new Result
            {
                Data = null,
                Command = eCommands.LRS,
                eCommandResult = eCommandResult.Damaged
            };
        }

        var quadrants = new List<QuadrantDTO>();
        var StarshipQX = Galaxy.CurrentQuadrant.Coord.X;
        var StarshipQY = Galaxy.CurrentQuadrant.Coord.Y;

        //Console.WriteLine(getCoord(Galaxy.CurrentQuadrant.Coord));


        for (int tempX = StarshipQX - Global.LRSRange; tempX <= StarshipQX + Global.LRSRange; tempX++)
        {
            for (int tempY = StarshipQY - Global.LRSRange; tempY <= StarshipQY + Global.LRSRange; tempY++)
            {
                var tempQuad = Galaxy.GetQuadrantByCoord(new Coord(tempX, tempY));
                if (tempQuad == null) continue;
                tempQuad.HasBeenExplored = true;
                quadrants.Add(tempQuad.toDTO);
            }
        }
        return new Result
        {
            Data = new Result.LRS(quadrants),
            Command = eCommands.LRS,
            eCommandResult = eCommandResult.OK
        };
    }

    public Result COM_REC()
    {
        if (Galaxy.Starship.getDeviceDamage(eDevice.COM) < 0)
            return new Result
            {
                Command = eCommands.COM_REC,
                eCommandResult = eCommandResult.Damaged,
                Data = null
            };
        return new Result
        {
            Command = eCommands.COM_REC,
            eCommandResult = eCommandResult.OK,
            Data = new Result.COM_REC(Galaxy.toQuadrantDTO.Where(z => z.HasBeenExplored).ToList())
        };
    }

    public Result COM_STA()
    {
        if (Galaxy.Starship.getDeviceDamage(eDevice.COM) < 0)
            return new Result
            {
                Data = null,
                Command = eCommands.COM_STA,
                eCommandResult = eCommandResult.Damaged
            };

        return new Result
        {
            Command = eCommands.COM_STA,
            eCommandResult = eCommandResult.OK,
            Data = new Result.COM_STA(
                Galaxy.MissionTime,
                Galaxy.CurrentQuadrant.Coord.toDTO,
                Galaxy.CurrentQuadrant.GetSectorByContents(Global.SectorStarship)?.Coord.toDTO ?? throw new NullReferenceException("Starship not found in current quadrant"),
                Galaxy.Starship.toDTO.Devices,
                Galaxy.getEnemiesRemaining(),
                Galaxy.Quadrants.Count(z => z.HasStarBase),
                Galaxy.Starship.EnergyLevel,
                Galaxy.Starship!.ShieldLevel,
                Galaxy.Starship.TorpedosRemaining,
                Galaxy.Starship.isDocked,
                Galaxy.MissionTimeDeadLine
                )
        };
    }

    public Result COM_STB()
    {
        if (Galaxy.Starship.getDeviceDamage(eDevice.COM) < 0)
            return new Result
            {
                Command = eCommands.COM_STB,
                eCommandResult = eCommandResult.Damaged,
                Data = null
            };

        var starbaseSector = Galaxy.CurrentQuadrant.GetSectorByContents(Global.SectorStarBase);
        if (starbaseSector == null)
            return new Result
            {
                Command = eCommands.COM_STB,
                eCommandResult = eCommandResult.CPU_STB_No_Starbase_Present,
                Data = null
            };

        var starshipSector = Galaxy.CurrentQuadrant.GetSectorByContents(Global.SectorStarship);
        if (starshipSector == null) return new Result();

        return new Result
        {
            Command = eCommands.COM_STB,
            eCommandResult = eCommandResult.OK,
            Data = new Result.COM_STB(
                Utils.CalculationUtilitiesV2.getDirection(starshipSector.Coord, starbaseSector.Coord),
                Utils.CalculationUtilitiesV2.getDistance(starshipSector.Coord, starbaseSector.Coord),
                starbaseSector.Coord.toDTO)
        };
    }

    public Result COM_TOR()
    {
        if (Galaxy.Starship.getDeviceDamage(eDevice.COM) < 0)
            return new Result
            {
                Command = eCommands.COM_TOR,
                eCommandResult = eCommandResult.Damaged,
                Data = null
            };

        if (Galaxy.CurrentQuadrant == null) return new Result();
        if (Galaxy.CurrentQuadrant.Sectors == null) return new Result();

        var enemySectors = Galaxy.CurrentQuadrant.Sectors.Where(z => z.SectorContents == Global.SectorEnemy).ToList();
        if (enemySectors.Count == 0)
            return new Result
            {
                Command = eCommands.COM_TOR,
                eCommandResult = eCommandResult.No_Enemies_Present,
                Data = null
            };

        List<DirectionToEnemyDTO> enemyLocations = new();
        var starshipSector = Galaxy.CurrentQuadrant.Sectors.First(z => z.SectorContents == Global.SectorStarship);

        foreach (var item in enemySectors)
        {
            var dir = Utils.CalculationUtilitiesV2.getDirection(starshipSector.Coord, item.Coord);
            var dist = Utils.CalculationUtilitiesV2.getDistance(starshipSector.Coord, item.Coord);
            var el = new DirectionToEnemyDTO(dir, dist, item.Coord.toDTO);
            enemyLocations.Add(el);
        }

        return new Result
        {
            Command = eCommands.COM_TOR,
            eCommandResult = eCommandResult.OK,
            Data = new Result.COM_TOR(enemyLocations)
        };
    }

    public Result COM_NAV(CoordDTO destQuadrant, CoordDTO destSector)
    {
        if (Galaxy.Starship.getDeviceDamage(eDevice.COM) < 0)
            return new Result
            {
                Command = eCommands.COM_NAV,
                eCommandResult = eCommandResult.Damaged,
                Data = null
            };

        if (Galaxy.CurrentQuadrant == null) return new Result();
        if (Galaxy.CurrentQuadrant.Coord == null) return new Result();
        if (Galaxy.CurrentQuadrant.Sectors == null) return new Result();

        var starshipSector = Galaxy.CurrentQuadrant.Sectors.First(z => z.SectorContents == Global.SectorStarship);

        var startUNIVx = CalculationUtilitiesV2.QS2Univ(Galaxy.CurrentQuadrant.Coord.X, starshipSector.Coord.X);
        var startUNIVy = CalculationUtilitiesV2.QS2Univ(Galaxy.CurrentQuadrant.Coord.Y, starshipSector.Coord.Y);
        var targetUNIVx = CalculationUtilitiesV2.QS2Univ(destQuadrant.X, destSector.X);
        var targetUNIVy = CalculationUtilitiesV2.QS2Univ(destQuadrant.Y, destSector.Y);

        var DIR = CalculationUtilitiesV2.getDirection(new Coord(startUNIVx, startUNIVy), new Coord(targetUNIVx, targetUNIVy));
        var DIST = CalculationUtilitiesV2.getDistance(new Coord(startUNIVx, startUNIVy), new Coord(targetUNIVx, targetUNIVy)) / 8;

        return new Result
        {
            Command = eCommands.COM_NAV,
            eCommandResult = eCommandResult.OK,
            Data = new Result.COM_NAV(DIR, DIST)
        };
    }

    public Result GetDamageReport()
    {
        if (Galaxy.Starship == null) return new Result();
        if (Galaxy.Starship.Devices == null) return new Result();

        return new Result
        {
            Command = eCommands.DAM,
            eCommandResult = eCommandResult.OK,
            Data = new Result.DAM(Galaxy.Starship.toDTO.Devices.Where(z => z.DamageLevel < 0).ToList())
        };
    }

    public Result SHE(int newShieldLevel)
    {
        if (newShieldLevel < 0)
        {
            return new Result
            {
                Command = eCommands.SHE,
                eCommandResult = eCommandResult.Error,
                Data = new Result.SHE(Global.Input_ErrorMessage)
            };
        }
        if (Galaxy.Starship.getDeviceDamage(eDevice.SHE) < 0)
            return new Result
            {
                Command = eCommands.SHE,
                eCommandResult = eCommandResult.Damaged,
                Data = null
            };

        if (Galaxy.Starship == null) return new Result();

        if (newShieldLevel > Galaxy.Starship.EnergyLevel + Galaxy.Starship.ShieldLevel)
        {
            return new Result
            {
                Command = eCommands.SHE,
                eCommandResult = eCommandResult.Error,
                Data = new Result.SHE(Global.SHE_ErrorMessage)
            };

        }

        var newEnergyLevel = Galaxy.Starship.EnergyLevel + Galaxy.Starship.ShieldLevel - newShieldLevel;
        Galaxy.Starship.EnergyLevel = newEnergyLevel;
        Galaxy.Starship.ShieldLevel = newShieldLevel;

        return new Result
        {
            Command = eCommands.SHE,
            Data = new Result.SHE(string.Empty),
            eCommandResult = eCommandResult.OK
        };
    }

    public Result TOR(double DIR)
    {
        if (Galaxy.Starship.getDeviceDamage(eDevice.TOR) < 0)
            return new Result
            {
                Command = eCommands.TOR,
                eCommandResult = eCommandResult.Damaged,
                Data = null
            };

        if (!double.IsFinite(DIR))
        {
            return new Result
            {
                Command = eCommands.TOR,
                eCommandResult = eCommandResult.Error,
                Data = null
            };
        }

        DIR = Math.Round(DIR, 1);

        if (DIR < .1 || DIR >= 9)
        {
            return new Result
            {
                Command = eCommands.TOR,
                eCommandResult = eCommandResult.Error,
                Data = null
            };
        }

        if (Galaxy.CurrentQuadrant == null) return new Result();
        if (Galaxy.CurrentQuadrant.Sectors == null) return new Result();
        if (Galaxy.CurrentQuadrant.Coord == null) return new Result();
        if (Galaxy.Starship == null) return new Result();

        var enemySectors = Galaxy.CurrentQuadrant.Sectors.Where(z => z.SectorContents == Global.SectorEnemy).ToList();
        if (enemySectors.Count == 0)
            return new Result
            {
                Command = eCommands.TOR,
                eCommandResult = eCommandResult.No_Enemies_Present,
                Data = null
            };

        if (Galaxy.Starship.TorpedosRemaining <= 0)
            return new Result
            {
                Command = eCommands.TOR,
                eCommandResult = eCommandResult.InsufficientInventory,
                Data = null
            };

        Galaxy.Starship.TorpedosRemaining -= 1;

        var starshipSector = Galaxy.CurrentQuadrant.Sectors.First(z => z.SectorContents == Global.SectorStarship);
        var CFO = CalculationUtilitiesV2.ScanForObject(Galaxy.CurrentQuadrant, starshipSector.Coord, DIR, false);
        //var CFO = CalculationUtilitiesV2.detectObject(Galaxy.CurrentQuadrant, starshipSector.Coord, false, DIR);

        if (CFO.ObjectHit == Global.SectorEmpty)
            return new Result
            {
                Command = eCommands.TOR,
                eCommandResult = eCommandResult.TOR_Missed,
                Data = null
            };

        if (CFO.ObjectHit == Global.SectorEnemy)
        {
            var targetSector = Galaxy.CurrentQuadrant.GetSectorByCoord(CFO.FinalCoord!);
            if (targetSector == null) return new Result();
            targetSector.SectorContents = Global.SectorEmpty;
            targetSector.Enemy = null;
            var currentQuadrant = Galaxy.GetQuadrantByCoord(Galaxy.CurrentQuadrant.Coord);
            if (currentQuadrant == null) return new Result();

            currentQuadrant.NumberOfEnemies -= 1;
            return new Result
            {
                Command = eCommands.TOR,
                eCommandResult = eCommandResult.OK,
                Data = new Result.TOR(new ObjectHitReportDTO(eObjectHit.Enemy, true, CFO.FinalCoord!.toDTO, -1, -1, false, false))
            };
        }

        if (CFO.ObjectHit == Global.SectorStarBase)
        {
            var targetSector = Galaxy.CurrentQuadrant.GetSectorByCoord(CFO.FinalCoord!);
            if (targetSector == null) return new Result();
            targetSector.SectorContents = Global.SectorEmpty;
            var currentQuadrant = Galaxy.GetQuadrantByCoord(Galaxy.CurrentQuadrant.Coord);
            if (currentQuadrant == null) return new Result();
            currentQuadrant.HasStarBase = false;

            return new Result
            {
                Command = eCommands.TOR,
                eCommandResult = eCommandResult.OK,
                Data = new Result.TOR(new(eObjectHit.Starbase, true, CFO.FinalCoord!.toDTO, -1, -1, false, false))
            };
        }

        if (CFO.ObjectHit == Global.SectorStar)
        {
            if (_randomFactory == null) return new Result();

            if (_randomFactory.GetRandomInteger(eRandomIntType.StarDestroyedChance) > Global.StarDestroyedChance)
            {
                var targetSector = Galaxy.CurrentQuadrant.GetSectorByCoord(CFO.FinalCoord!);
                if (targetSector == null) return new Result();
                targetSector.SectorContents = Global.SectorEmpty;
                var currentQuadrant = Galaxy.GetQuadrantByCoord(Galaxy.CurrentQuadrant.Coord);
                if (currentQuadrant == null) return new Result();
                currentQuadrant.NumberOfStars -= 1;

                return new Result
                {
                    Command = eCommands.TOR,
                    eCommandResult = eCommandResult.OK,
                    Data = new Result.TOR(new(eObjectHit.Star, true, CFO.FinalCoord!.toDTO, -1, -1, false, false))
                };
            }
            else
            {
                return new Result
                {
                    Command = eCommands.TOR,
                    eCommandResult = eCommandResult.OK,
                    Data = new Result.TOR(new(eObjectHit.Star, false, CFO.FinalCoord!.toDTO, -1, -1, false, true))
                };
            }
        }

        return new Result();
    }

    public Result LAS(int energy)
    {
        if (energy <= 0)
        {
            return new Result
            {
                Command = eCommands.LAS,
                eCommandResult = eCommandResult.Error,
                Data = null
            };
        }

        if (Galaxy.Starship.getDeviceDamage(eDevice.LAS) < 0)
            return new Result
            {
                Command = eCommands.LAS,
                eCommandResult = eCommandResult.Damaged,
                Data = null
            };

        if (Galaxy.Starship.EnergyLevel < energy)
            return new Result
            {
                Command = eCommands.LAS,
                eCommandResult = eCommandResult.InsufficientInventory,
                Data = null
            };

        var enemySectors = Galaxy.CurrentQuadrant.Sectors.Where(z => z.SectorContents == Global.SectorEnemy).ToList();
        if (enemySectors.Count == 0)
            return new Result
            {
                Command = eCommands.LAS,
                eCommandResult = eCommandResult.No_Enemies_Present,
                Data = null
            };

        var starshipSector = Galaxy.CurrentQuadrant.Sectors.First(z => z.SectorContents == Global.SectorStarship);
        var enemyHitReports = new List<ObjectHitReportDTO>();
        Galaxy.Starship.EnergyLevel -= energy;
        foreach (var enemySector in enemySectors)
        {
            if (enemySector == null) continue;
            if (enemySector.Enemy == null) continue;

            //If CPU damaged, then accuracy isn't as good.
            if (Galaxy.Starship.getDeviceDamage(eDevice.COM) < 0 &&
                _randomFactory.GetRandomInteger(eRandomIntType.LaserMissChance) > Global.LaserMissChanceCPUDown)
            {
                enemyHitReports.Add(new ObjectHitReportDTO(eObjectHit.Enemy, false, enemySector.Coord.toDTO, enemySector.Enemy.ShieldLevel, 0, true, false));
            }
            else
            {
                var DIST2Enemy = CalculationUtilitiesV2.getDistance(starshipSector.Coord, enemySector.Coord);
                var EnergyHitEnemy = CalculationUtilitiesV2.EnemyShieldHit(enemySectors.Count, energy, DIST2Enemy, Global.Sector2EnergyConversion);
                enemySector.Enemy.ShieldLevel = enemySector.Enemy.ShieldLevel - EnergyHitEnemy;

                if (enemySector.Enemy.ShieldLevel <= 0)
                {
                    var targetSector = Galaxy.CurrentQuadrant.GetSectorByCoord(enemySector.Coord);
                    if (targetSector == null) return new Result();
                    targetSector.SectorContents = Global.SectorEmpty;
                    targetSector.Enemy = null;
                    var currentQuadrant = Galaxy.GetQuadrantByCoord(Galaxy.CurrentQuadrant.Coord);
                    if (currentQuadrant == null) return new Result();
                    currentQuadrant.NumberOfEnemies -= 1;

                    enemyHitReports.Add(new ObjectHitReportDTO(eObjectHit.Enemy, true, enemySector.Coord.toDTO, -1, EnergyHitEnemy, false, false));
                }
                else
                {
                    enemyHitReports.Add(new ObjectHitReportDTO(eObjectHit.Enemy, false, enemySector.Coord.toDTO, enemySector.Enemy.ShieldLevel, EnergyHitEnemy, false, false));
                }
            }
        }

        return new Result
        {
            Command = eCommands.LAS,
            eCommandResult = eCommandResult.OK,
            Data = new Result.LAS(enemyHitReports)
        };
    }

    public Result NAV(double DIR, double DIST)
    {
        if (!double.IsFinite(DIR))
        {
            return new Result
            {
                Command = eCommands.NAV,
                eCommandResult = eCommandResult.Error,
                Data = new Result.NAV(eNavMessage.InvalidDIR, -1)
            };
        }

        if (!double.IsFinite(DIST))
        {
            return new Result
            {
                Command = eCommands.NAV,
                eCommandResult = eCommandResult.Error,
                Data = new Result.NAV(eNavMessage.InvalidDIST, -1)
            };
        }

        DIR = Math.Round(DIR, 1);
        DIST = Math.Round(DIST, 1);

        if (DIR < .1 || DIR >= 9)
        {
            return new Result
            {
                Command = eCommands.NAV,
                eCommandResult = eCommandResult.Error,
                Data = new Result.NAV(eNavMessage.InvalidDIR, -1)
            };
        }

        if (DIST < .1 || DIST > 8)
        {
            return new Result
            {
                Command = eCommands.NAV,
                eCommandResult = eCommandResult.Error,
                Data = new Result.NAV(eNavMessage.InvalidDIST, -1)
            };
        }


        if (DIR < .1 || DIR >= 9)
            return new Result
            {
                Command = eCommands.NAV,
                eCommandResult = eCommandResult.Error,
                Data = new Result.NAV(eNavMessage.InvalidDIR, -1)
            };
        if (DIST < .1 || DIST > 8)
            return new Result
            {
                Command = eCommands.NAV,
                eCommandResult = eCommandResult.Error,
                Data = new Result.NAV(eNavMessage.InvalidDIST, -1)
            };

        if (Galaxy.Starship.getDeviceDamage(eDevice.NAV) < 0 && DIST > 0.2)
            return new Result
            {
                Command = eCommands.NAV,
                eCommandResult = eCommandResult.Damaged,
                Data = null
            };

        var starshipSector = Galaxy.CurrentQuadrant.Sectors!.First(z => z.SectorContents == Global.SectorStarship);

        var FDO = CalculationUtilitiesV2.getFinalDestination(DIR, DIST, starshipSector, Galaxy.CurrentQuadrant);
        if (FDO.OutsideGalaxy)
            return new Result
            {
                eCommandResult = eCommandResult.Error,
                Command = eCommands.NAV,
                Data = new Result.NAV(eNavMessage.BadInput_OutsideGalaxy, -1)
            };

        int EstimateAmountOfEnergyForTrip = CalculationUtilitiesV2.Distance2Energy(Galaxy.CurrentQuadrant.Coord, starshipSector.Coord, FDO.FinalQuadrantCoord!, FDO.FinalSectorCoord!);

        Coord StartingStarshipSectorCoord = starshipSector.Coord;
        Coord StartingStarshipQuadrantCoord = Galaxy.CurrentQuadrant.Coord;

        if (Galaxy.Starship.EnergyLevel < EstimateAmountOfEnergyForTrip)
            if ((Galaxy.Starship.EnergyLevel + Galaxy.Starship.ShieldLevel) > EstimateAmountOfEnergyForTrip)
                return new Result
                {
                    Command = eCommands.NAV,
                    eCommandResult = eCommandResult.Error,
                    Data = new Result.NAV(eNavMessage.InsufficientEnergy_ShieldEnergyAvailable, -1)
                };
            else
                return new Result
                {
                    Command = eCommands.NAV,
                    eCommandResult = eCommandResult.Error,
                    Data = new Result.NAV(eNavMessage.InsufficientEnergy, -1)
                };

        var navMessage = Galaxy.CheckRouteAndMoveStarship(_randomFactory, _currentQuadrantFactory, DIR, DIST, EstimateAmountOfEnergyForTrip, starshipSector.Coord, FDO.FinalQuadrantCoord!, FDO.FinalSectorCoord!);
        var distTraveled = CalculationUtilitiesV2.getDistance(starshipSector.Coord, FDO.FinalSectorCoord!);
        if (FDO.ObjectHit)
            return new Result
            {
                Command = eCommands.NAV,
                eCommandResult = eCommandResult.Error,
                Data = new Result.NAV(eNavMessage.BadInput_ObjectHit, distTraveled)
            };


        if (StartingStarshipQuadrantCoord == FDO.FinalQuadrantCoord)
            distTraveled = CalculationUtilitiesV2.getDistance(StartingStarshipSectorCoord, FDO.FinalSectorCoord!);
        else
            distTraveled = CalculationUtilitiesV2.getDistance(StartingStarshipQuadrantCoord, FDO.FinalQuadrantCoord!) * 8;


        return new Result
        {
            Command = eCommands.NAV,
            eCommandResult = eCommandResult.OK,
            Data = new Result.NAV(navMessage, distTraveled)
        };
    }

    public Result RunRoutineMaintenance(double changeMissionTime)
    {
        var newMissionTime = UpdateStartdate(changeMissionTime);

        if (Galaxy.Starship == null) return new Result();
        if (Galaxy.Starship.Devices == null) return new Result();
        if (Galaxy.Quadrants == null) return new Result();

        var DockingStatus = new DockingStatusArgDTO(Galaxy.Starship.isDocked, false);

        Galaxy.Starship.isDocked = false;

        if (Galaxy.Starship.ShieldLevel == 0 && canBeDocked())
        {
            DockingStatus.CurrentStatus = true;
            Galaxy.Starship.isDocked = true;
            Galaxy.Starship.EnergyLevel = Global.maxStarshipEnergy;
            Galaxy.Starship.TorpedosRemaining = Global.maxStarshipTorpedos;
        }

        var enemiesMoved = EnemiesMoved();
        var repairedDevices = Repairs(changeMissionTime);
        var enemiesFired = EnemiesFire();

        var gameStatus = eGameStatus.Normal;

        if (newMissionTime > Galaxy.MissionTimeDeadLine)
            gameStatus = eGameStatus.RanOutOfTime;
        else if (Galaxy.Starship.isDestroyed)
            gameStatus = eGameStatus.StarshipDestroyed;
        else if (Galaxy.Starship.EnergyLevel == 0 && Galaxy.Starship.ShieldLevel == 0)
            gameStatus = eGameStatus.RanOutOfEnergy;
        else if (Galaxy.Starship.EnergyLevel == 0 && Galaxy.Starship.ShieldLevel > 0 && Galaxy.Starship.Devices.First(z => z.Name == Galaxy.Starship.getDevice(eDevice.SHE)).DamageLevel >= 0)
            gameStatus = eGameStatus.OutOfEnergyShieldEnergyAvailable;
        else if (Galaxy.Starship.EnergyLevel == 0 && Galaxy.Starship.ShieldLevel > 0 && Galaxy.Starship.Devices.First(z => z.Name == Galaxy.Starship.getDevice(eDevice.SHE)).DamageLevel < 0)
            gameStatus = eGameStatus.RanOutOfEnergy;
        else if (Galaxy.Quadrants.Sum(z => z.NumberOfEnemies) == 0)
            gameStatus = eGameStatus.MissionOver;

        if (Galaxy.Starship.isDestroyed) gameStatus = eGameStatus.StarshipDestroyed;


        bool anyEnemiesInQuadrant = Galaxy.CurrentQuadrant.GetSectorByContents(Global.SectorEnemy)?.HasEnemy ?? false;
        int entShieldLevel = Galaxy.Starship.ShieldLevel;

        return new Result
        {
            eCommandResult = eCommandResult.OK,
            Command = eCommands.MaintResult,
            Data = new Result.MaintResult(
                newMissionTime,
                repairedDevices,
                enemiesFired,
                enemiesMoved,
                DockingStatus,
                StarbaseRepairs(),
                gameStatus,
                anyEnemiesInQuadrant,
                entShieldLevel)
        };
    }

    public void ExecuteStarbaseRepairs(double MissionTimeUnits)
    {
        if (Galaxy.Starship == null) return;
        if (Galaxy.Starship.Devices == null) return;

        Galaxy.MissionTime += Math.Abs(MissionTimeUnits);
        foreach (var Device in Galaxy.Starship.Devices.Where(z => z.DamageLevel < 0).ToList())
        {
            Device.DamageLevel = 0;
        }
    }

    private double UpdateStartdate(double changeMissionTime)
    {
        Galaxy.MissionTime += changeMissionTime;
        return Galaxy.MissionTime;
    }

    private List<DeviceDTO> Repairs(double changeMissionTime)
    {
        if (Galaxy.Starship == null) return new List<DeviceDTO>();
        if (Galaxy.Starship.Devices == null) return new List<DeviceDTO>();

        var DamagedDevices = Galaxy.Starship.Devices.Where(z => z.DamageLevel < 0).ToList();

        var devsOut = new List<DeviceDTO>();

        foreach (var DD in DamagedDevices)
        {
            DD.DamageLevel = DD.DamageLevel + changeMissionTime;
            DD.DamageLevel = Math.Round(DD.DamageLevel, 1);
            if (Math.Round(DD.DamageLevel, 1) >= 0) DD.DamageLevel = 100;
            devsOut.Add(DD.toDTO);
        }
        return devsOut;
    }

    private List<EnemyMovedDTO>? EnemiesMoved()
    {
        if (Galaxy.skipEnemyMoveSW) return null;
        if (Galaxy.CurrentQuadrant == null) return null;
        if (Galaxy.CurrentQuadrant.Sectors == null) return null;
        if (Galaxy.Starship == null) return null;
        if (Galaxy.Starship.Devices == null) return null;

        var EnemySectorsInQuadrant = Galaxy.CurrentQuadrant.Sectors.Where(z => z.SectorContents == Global.SectorEnemy).ToList();
        if (EnemySectorsInQuadrant.Count == 0) return null;
        var OutList = new List<EnemyMovedDTO>();

        foreach (var enemySector in EnemySectorsInQuadrant)
        {
            if (enemySector == null) continue;
            if (enemySector.Enemy == null) continue;
            var enemyMoveChance = _randomFactory.GetRandomInteger(eRandomIntType.EnemyMoveChance);
            if (enemyMoveChance == 0 && !Galaxy.forceEnemyMoveSW) continue;
            if (enemyMoveChance < Global.EnemyMoveChance && !Galaxy.forceEnemyMoveSW) continue;

            Debug.WriteLine(Global.EnemyMoveChance);

            var MT_Sector = _currentQuadrantFactory.getRandomEmptySector(_randomFactory, Galaxy.CurrentQuadrant.Sectors, eRandomCoordType.EnemyLocation);


            MT_Sector.SectorContents = enemySector.SectorContents;
            MT_Sector.Enemy = new(enemySector.Enemy.ShieldLevel);

            enemySector.Enemy = null;
            enemySector.SectorContents = Global.SectorEmpty;

            OutList.Add(new(enemySector.Coord.toDTO, MT_Sector.Coord.toDTO));
        }

        return OutList;
    }

    private List<EnemyFiredDTO>? EnemiesFire()
    {
        if (Galaxy.CurrentQuadrant == null) return null;
        if (Galaxy.CurrentQuadrant.Sectors == null) return null;
        if (Galaxy.Starship == null) return null;
        if (Galaxy.Starship.Devices == null) return null;

        var EnemySectorsInQuadrant = Galaxy.CurrentQuadrant.Sectors.Where(z => z.SectorContents == Global.SectorEnemy).ToList();
        if (EnemySectorsInQuadrant.Count == 0) return null;
        var OutList = new List<EnemyFiredDTO>();

        if (Galaxy.stopEnemyFireSW)
        {
            //Console.WriteLine("\n\n\n\nSKIPPING ENEMY FIRE NOW!!\n\n\n\n");
            return OutList;
        }

        //throw new Exception("\n\n\n\n\n\n\n\nHERE\n\n\n\n\n\n\n\n");

        foreach (var enemySector in EnemySectorsInQuadrant)
        {
            if (enemySector == null) continue;
            if (enemySector.Enemy == null) continue;

            var Enemy = enemySector.Enemy;
            var EnemyHitAmount = _randomFactory.GetRandomInteger(eRandomIntType.EnemyFiredAmount);


            if (Galaxy.Starship.isDocked)
            {
                var EnemyFired = new EnemyFiredDTO(
                            enemySector.Coord.toDTO,
                            Galaxy.Starship.ShieldLevel,
                            Galaxy.Starship.isDestroyed,
                            null,
                            0,
                            EnemyHitAmount,
                            true);
                OutList.Add(EnemyFired);
            }
            else
            {
                Galaxy.Starship.ShieldLevel -= EnemyHitAmount;

                if (Galaxy.Starship.ShieldLevel < 0)
                {
                    Galaxy.Starship.isDestroyed = true;

                    var EnemyFired = new EnemyFiredDTO(
                        enemySector.Coord.toDTO,
                        Galaxy.Starship.ShieldLevel,
                        Galaxy.Starship.isDestroyed,
                        null,
                        0,
                        EnemyHitAmount,
                        false);
                    OutList.Add(EnemyFired);
                }
                else
                {
                    Device? damagedDevice = null;
                    int damagedDeviceMissionTimeUnits = 0;

                    if (_randomFactory.GetRandomInteger(eRandomIntType.DamagedDeviceChance)
                        > Global.DamageDeviceRandomNumberThreshold)
                    {
                        for (int attempt = 0; attempt < 10; attempt++)
                        {
                            int deviceNumber =
                                _randomFactory.GetRandomInteger(eRandomIntType.DamagedDevice);

                            var candidate = Galaxy.Starship.Devices[deviceNumber];

                            if (candidate.DamageLevel >= 0)
                            {
                                damagedDevice = candidate;
                                break;
                            }
                        }

                        damagedDevice ??=
                            Galaxy.Starship.Devices.FirstOrDefault(
                                device => device.DamageLevel >= 0);

                        if (damagedDevice is not null)
                        {
                            damagedDeviceMissionTimeUnits =
                                _randomFactory.GetRandomInteger(
                                    eRandomIntType.DamagedDeviceAmount) * -1;

                            damagedDevice.DamageLevel = damagedDeviceMissionTimeUnits;
                        }
                    }

                    var EnemyFired = new EnemyFiredDTO(
                        enemySector.Coord.toDTO,
                        Galaxy.Starship.ShieldLevel,
                        Galaxy.Starship.isDestroyed,
                        damagedDevice?.toDTO,
                        damagedDeviceMissionTimeUnits,
                        EnemyHitAmount,
                        false);
                    OutList.Add(EnemyFired);
                }
            }
        }
        return OutList;
    }

    private double? StarbaseRepairs()
    {
        if (Galaxy.Starship == null) return null;
        if (Galaxy.Starship.Devices == null) return null;

        if (!Galaxy.Starship.isDocked) return null;

        var DamagedDevices = Galaxy.Starship.Devices.Where(z => z.DamageLevel < 0).ToList();

        if (DamagedDevices.Count == 0) return null;

        double MissionTimeUnitsForRepair = Galaxy.Starship.Devices.Min(z => z.DamageLevel);  // use MIN because we're dealing with negatives
        return MissionTimeUnitsForRepair;
    }

    private bool canBeDocked()
    {
        var sectorStarship = Galaxy.CurrentQuadrant.GetSectorByContents(Global.SectorStarship);
        if (sectorStarship == null) return false;

        (var resultTopLeft, var resultBottomRight) = CalculationUtilitiesV2.getCoordRange(sectorStarship.Coord);

        for (int X = resultTopLeft.X; X <= resultBottomRight.X; X++)
        {
            for (int Y = resultTopLeft.Y; Y <= resultBottomRight.Y; Y++)
            {
                var tempSector = Galaxy.CurrentQuadrant.GetSectorByCoord(new Coord(X, Y));
                if (tempSector == null) continue;

                if (tempSector.SectorContents == Global.SectorStarBase)
                {
                    return true;
                }
            }
        }
        return false;
    }
}