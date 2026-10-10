using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library;
using RobsSpacePatrol.Library.eNums;
using RobsSpacePatrol.Library.Factories.Interfaces;
using RobsSpacePatrol.Library.Models;

namespace RobsSpacePatrol.Factories;

public class CurrentQuadrantFactory : ICurrentQuadrantFactory
{
    public Sector getRandomEmptySector(IRandomFactory randomFactory, List<Sector> sectors, eRandomCoordType randomCoordType)
    {
        Coord coord;
        Sector ChosenSector;
        int LoopCount = 0;

        do
        {
            coord = randomFactory.GetRandomCoord(randomCoordType) ?? throw new ArgumentNullException($"RandomCoord returned null for {randomCoordType.ToString()}");

            ChosenSector = sectors.First(z => z.Coord == coord);

            LoopCount += 1;
            if (LoopCount > 15)
            {
                ChosenSector = sectors.First(z => z.IsEmpty);  //after 15 attempts, just pick the first one to return;
            }
        } while (ChosenSector.SectorContents != Global.SectorEmpty);

        return ChosenSector;
    }
    private Sector createEmptySector(int x, int y)
    {
        return new Sector(new Coord(x, y), Global.SectorEmpty);
    }
    public CurrentQuadrant createCurrentQuadrant(Quadrant quadrant, IRandomFactory randomFactory, Coord starshipSectorCoord)
    {
        var sectors = new List<Sector>();

        for (int x = 0; x < Global.MaxSectorsXY; x++)
            for (int y = 0; y < Global.MaxSectorsXY; y++)
                sectors.Add(createEmptySector(x, y));

        var starshipSector = sectors.First(z => z.Coord == starshipSectorCoord);
        starshipSector.SectorContents = Global.SectorStarship;

        for (int i = 0; i < quadrant.NumberOfStars; i++)
        {
            var emptySector = getRandomEmptySector(randomFactory, sectors, eRandomCoordType.StarLocation);
            emptySector.SectorContents= Global.SectorStar;
        }

        if (quadrant.HasStarBase)
        {
            var emptySector = getRandomEmptySector(randomFactory, sectors, eRandomCoordType.StarBaseLocation);
            emptySector.SectorContents = Global.SectorStarBase;
        }

        for (int i = 0; i < quadrant.NumberOfEnemies; i++)
        {
            var sect = getRandomEmptySector(randomFactory, sectors, eRandomCoordType.EnemyLocation);
            sect.SectorContents = Global.SectorEnemy;
            sect.Enemy = new Enemy(randomFactory.GetRandomInteger(eRandomIntType.EnemyShieldLevel));
        }

        quadrant.HasBeenExplored = true;

        return new CurrentQuadrant(quadrant.Coord, sectors);
    }
}
