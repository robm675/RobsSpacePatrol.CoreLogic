using RandomGen;
using RobsSpacePatrol.Library.Factories.Interfaces;
using RobsSpacePatrol.Library.eNums;
using RobsSpacePatrol.Library.Models;
using RobsSpacePatrol.Library;
using RobsSpacePatrol.DTO.Library;

namespace RobsSpacePatrol.Factories;


public class RandomFactory : IRandomFactory
{
    private readonly Random _random;

    public RandomFactory(Random? random = null)
    {
        _random = random ?? Random.Shared;
    }

    public Coord? GetRandomCoord(eRandomCoordType randomType)
    {
        switch (randomType)
        {
            case eRandomCoordType.StarshipSector:
                return new Coord(GetRandomInteger(0, Global.MaxSectorsXY - 1), GetRandomInteger(0, Global.MaxSectorsXY - 1));
            case eRandomCoordType.StarshipQuadrant:
                return new Coord(GetRandomInteger(0, Global.MaxSectorsXY - 1), GetRandomInteger(0, Global.MaxSectorsXY - 1));
            case eRandomCoordType.StarLocation:
                return new Coord(GetRandomInteger(0, Global.MaxSectorsXY - 1), GetRandomInteger(0, Global.MaxSectorsXY - 1));
            case eRandomCoordType.EnemyLocation:
                return new Coord(GetRandomInteger(0, Global.MaxSectorsXY - 1), GetRandomInteger(0, Global.MaxSectorsXY - 1));
            case eRandomCoordType.StarBaseLocation:
                return new Coord(GetRandomInteger(0, Global.MaxSectorsXY - 1), GetRandomInteger(0, Global.MaxSectorsXY - 1));
            default:
                return null;
        }
    }
    public int GetRandomInteger(eRandomIntType randomType)
    {
        switch (randomType)
        {
            case eRandomIntType.StartingMissionTime:
                return GetRandomInteger(2400, 2499);
            case eRandomIntType.EnemyMoveChance:
                return GetRandomInteger(0, 100);
            case eRandomIntType.EnemyFiredAmount:
                return GetRandomInteger(10, 50);
            case eRandomIntType.DamagedDevice:
                var devices = Enum.GetValues(typeof(eDevice));
                return GetRandomInteger(0, devices.Length - 1);
            case eRandomIntType.DamagedDeviceAmount:
                return GetRandomInteger(1, Global.MaxDamageMissionTimeAmount);
            case eRandomIntType.DamagedDeviceChance:
                return GetRandomInteger(0, 100);
            case eRandomIntType.EnemyChance:
                return GetRandomInteger(0, 100);
            case eRandomIntType.LaserMissChance:
                return GetRandomInteger(0, 100);
            case eRandomIntType.StarBaseChance:
                return GetRandomInteger(0, 100);
            case eRandomIntType.StarDestroyedChance:
                return GetRandomInteger(0, 100);
            case eRandomIntType.StarQuantity:
                return GetRandomInteger(0, Global.MaxStarsPerQuadrant);
            case eRandomIntType.EnemyShieldLevel:
                return GetRandomInteger(100, 200);
        }
        return -1;
    }
    private int GetRandomInteger(int min, int max)
    {
        return _random.Next(min, checked(max + 1));
    }
}
