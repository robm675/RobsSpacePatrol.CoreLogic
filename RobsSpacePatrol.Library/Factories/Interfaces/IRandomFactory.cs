using RobsSpacePatrol.Library.eNums;
using RobsSpacePatrol.Library.Models;

namespace RobsSpacePatrol.Library.Factories.Interfaces;

public interface IRandomFactory
{
    Coord? GetRandomCoord(eRandomCoordType randomType);
    int GetRandomInteger(eRandomIntType randomType);
}
