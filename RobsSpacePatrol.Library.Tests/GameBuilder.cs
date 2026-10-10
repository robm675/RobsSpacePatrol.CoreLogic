
using RobsSpacePatrol.Factories;
using RobsSpacePatrol.Library.Factories;
using RobsSpacePatrol.Library.Factories.Interfaces;
using RobsSpacePatrol.Library.Models;

namespace RobsSpacePatrol.CoreLogic.Tests
{
    public static class GameBuilder
    {
        public static Game getGame(RandomFactoryBuilder? randomFactoryBuilder = null)
        {
            IRandomFactory randomFactory;
            if (randomFactoryBuilder == null)
                randomFactory = new RandomFactory();
            else
                randomFactory = randomFactoryBuilder.GetFactories();

            var galaxyFactory = new GalaxyFactory();
            var currentQuadrantFactory = new CurrentQuadrantFactory();
            var quadrantFactory = new QuadrantFactory();

            var galaxy = galaxyFactory.createGalaxy(randomFactory, currentQuadrantFactory, quadrantFactory);
            var game = new Game(galaxy, randomFactory, currentQuadrantFactory);

            return game;
        }

        public static Game getGame(out Galaxy galaxy, RandomFactoryBuilder? randomFactoryBuilder = null)
        {
            IRandomFactory randomFactory;
            if (randomFactoryBuilder == null)
                randomFactory = new RandomFactory();
            else
                randomFactory = randomFactoryBuilder.GetFactories();

            var galaxyFactory = new GalaxyFactory();
            var currentQuadrantFactory = new CurrentQuadrantFactory();
            var quadrantFactory = new QuadrantFactory();

            galaxy = galaxyFactory.createGalaxy(randomFactory, currentQuadrantFactory, quadrantFactory);
            var game = new Game(galaxy, randomFactory, currentQuadrantFactory);

            return game;
        }
    }
}
