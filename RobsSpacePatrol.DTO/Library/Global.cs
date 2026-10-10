using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobsSpacePatrol.DTO.Library;

public record Global
{
    public const int MaxQuadrantsXY = 8;
    public const int MaxSectorsXY = 8;
    public const int MaxStarsPerQuadrant = 5;

    public const int EnemyZeroOne = 75;
    public const int EnemyOneTwo = 90;
    public const int EnemyTwoThree = 96;

    public const double MissionTimeUnitsPerEnemyRatio = 2.0;

    public const int StarbaseChance = 95;

    public const int EnemyMoveChance = 85;

    public const int LRSRange = 1;
    public const int maxStarshipEnergy = 3000;
    public const int maxStarshipTorpedos = 10;
    public const int Sector2EnergyConversion = 4;
    public const int LaserMissChanceCPUDown = 60;
    public const int StarDestroyedChance = 80;
    public const int DamageDeviceRandomNumberThreshold = 85;
    public const double Sector2TimeConversion = .1;
    public const int MaxDamageMissionTimeAmount = 4;

    public const string SectorEmpty = "EMPTY";
    public const string SectorStarship = "STARSHIP";
    public const string SectorStar = "STAR";
    public const string SectorStarBase = "STARBASE";
    public const string SectorEnemy = "ENEMY";

    public const string SHE_ErrorMessage = "Insufficient Energy";
    public const string Input_ErrorMessage = "Invalid Input";
}
