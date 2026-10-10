using RobsSpacePatrol.Library.eNums;
using RobsSpacePatrol.Library.Models;

namespace RobsSpacePatrol.Library.Factories.Interfaces;

public interface ICurrentQuadrantFactory
{
    CurrentQuadrant createCurrentQuadrant(Quadrant quadrant, IRandomFactory randomFactory, Coord starshipSectorCoord);
    Sector getRandomEmptySector(IRandomFactory randomFactory, List<Sector> sectors, eRandomCoordType randomCoordType);
}
