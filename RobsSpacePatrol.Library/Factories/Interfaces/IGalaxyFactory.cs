using RobsSpacePatrol.Library.Models;

namespace RobsSpacePatrol.Library.Factories.Interfaces;

public interface IGalaxyFactory
{
    Galaxy createGalaxy(IRandomFactory randomFactory, ICurrentQuadrantFactory currentQuadrantFactory, IQuadrantFactory quadrantFactory);
}
