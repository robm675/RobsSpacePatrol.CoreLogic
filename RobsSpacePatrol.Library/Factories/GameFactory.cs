using RobsSpacePatrol.Library.Factories.Interfaces;
using RobsSpacePatrol.Library.Game;

namespace RobsSpacePatrol.Library.Factories;

public static class GameFactory
{
    public static IGame getGame()
    {
        IRandomFactory randomFactory = new RandomFactory();

        var galaxyFactory = new GalaxyFactory();
        var currentQuadrantFactory = new CurrentQuadrantFactory();
        var quadrantFactory = new QuadrantFactory();

        var galaxy = galaxyFactory.createGalaxy(randomFactory, currentQuadrantFactory, quadrantFactory);
        var game = new Game.Game(galaxy, randomFactory, currentQuadrantFactory);

        return game;
    }
}
