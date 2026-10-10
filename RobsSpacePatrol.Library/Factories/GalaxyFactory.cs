using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library.eNums;
using RobsSpacePatrol.Library.Factories.Interfaces;
using RobsSpacePatrol.Library.Models;

namespace RobsSpacePatrol.Library.Factories;
public class GalaxyFactory : IGalaxyFactory
{
    public Galaxy createGalaxy(IRandomFactory randomFactory, ICurrentQuadrantFactory currentQuadrantFactory, IQuadrantFactory quadrantFactory)
    {
        List<Quadrant> quadrants = [];
        for (int xCount = 0; xCount < Global.MaxQuadrantsXY; xCount++)
            for (int yCount = 0; yCount < Global.MaxQuadrantsXY; yCount++)
                quadrants.Add(quadrantFactory.createQuadrant(new Coord(xCount, yCount), randomFactory));

        Coord StarshipQuadrantCoord = randomFactory.GetRandomCoord(eRandomCoordType.StarshipQuadrant) ?? throw new ArgumentNullException("Starship Quadrant RandomCoord returned null");
        Coord StarshipSectorCoord = randomFactory.GetRandomCoord(eRandomCoordType.StarshipSector) ?? throw new ArgumentNullException("Starship Sector RandomCoord returned null");

        Quadrant currentStarshipQuadrant = quadrants.First(z =>z.Coord == StarshipQuadrantCoord);

        var currentQuadrant = currentQuadrantFactory.createCurrentQuadrant(currentStarshipQuadrant, randomFactory, StarshipSectorCoord);

        quadrants.First(z => z.Coord == StarshipQuadrantCoord).HasBeenExplored = true;

        var starship = new Starship();

        var missionTime = randomFactory.GetRandomInteger(eRandomIntType.StartingMissionTime);

        return new Galaxy(starship, currentQuadrant, quadrants, missionTime);
    }
}
