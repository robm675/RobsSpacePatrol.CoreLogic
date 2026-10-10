
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Factories;
using RobsSpacePatrol.Library.eNums;
using RobsSpacePatrol.Library.Factories;
using RobsSpacePatrol.Library.Models;
using static RobsSpacePatrol.DTO.Library.Result;

namespace RobsSpacePatrol.CoreLogic.Tests
{
    public class COM_NAVTests
    {
        [Fact]
        public void test_COM_NAV_ResultNotNull()
        {
            var galaxy = new GalaxyFactory().createGalaxy(new RandomFactory(), new CurrentQuadrantFactory(), new QuadrantFactory());
            var game = new Game(galaxy, new RandomFactory(), new CurrentQuadrantFactory());
            var destQuadrant = new Coord(0, 1);
            var destSector = new Coord(0, 0);

            Assert.NotNull(game.COM_NAV(destQuadrant.toDTO, destSector.toDTO));
        }

        [Fact]
        public void test_COM_NAV_hasContents()
        {
            var galaxy = new GalaxyFactory().createGalaxy(new RandomFactory(), new CurrentQuadrantFactory(), new QuadrantFactory());
            var game = new Game(galaxy, new RandomFactory(), new CurrentQuadrantFactory());
            var destQuadrant = new Coord(0, 1);
            var destSector = new Coord(0, 0);

            var result = game.COM_NAV(destQuadrant.toDTO, destSector.toDTO);

            Assert.NotNull(result.getResults<COM_NAV>());
            Assert.Equal(eCommandResult.OK, result.eCommandResult);
        }

        [Fact]
        public void test_COM_NAV_damaged()
        {
            var galaxy = new GalaxyFactory().createGalaxy(new RandomFactory(), new CurrentQuadrantFactory(), new QuadrantFactory());
            var destQuadrant = new Coord(0, 1);
            var destSector = new Coord(0, 0);
            var game = new Game(galaxy, new RandomFactory(), new CurrentQuadrantFactory());

            game.Galaxy.Starship.setDeviceDamage(eDevice.COM, -3);

            var result = game.COM_NAV(destQuadrant.toDTO, destSector.toDTO);

            Assert.Null(result.getResults<COM_NAV>());
            Assert.Equal(eCommands.COM_NAV, result.Command);
            Assert.Equal(eCommandResult.Damaged, result.eCommandResult);
        }

        [Fact]
        public void test_COM_NAV_verifiedContents_X()
        {
            var rf = new RandomFactoryBuilder()
                .setStarshipQuadrant(new Coord(0,0))
                .setStarshipSector(new Coord(0, 0));

            var galaxy = new GalaxyFactory().createGalaxy(rf.GetFactories(), new CurrentQuadrantFactory(), new QuadrantFactory());
            var destQuadrant = new Coord (1, 0);
            var destSector = new Coord (0, 0);

            var game = new Game(galaxy, rf.GetFactories(), new CurrentQuadrantFactory());
            var result = game.COM_NAV(destQuadrant.toDTO, destSector.toDTO).getResults<COM_NAV>();

            Assert.Equal(1, result!.Distance);
            Assert.Equal(1, result.Direction);
        }

        [Fact]
        public void test_COM_NAV_verifiedContents_Y()
        {
            var rf = new RandomFactoryBuilder()
                .setStarshipQuadrant(new Coord(0, 0))
                .setStarshipSector(new Coord(0, 0));

            var galaxy = new GalaxyFactory().createGalaxy(rf.GetFactories(), new CurrentQuadrantFactory(), new QuadrantFactory());
            var destQuadrant = new Coord (0, 1);
            var destSector = new Coord (0, 0);

            var game = new Game(galaxy, rf.GetFactories(), new CurrentQuadrantFactory());
            var result = game.COM_NAV(destQuadrant.toDTO, destSector.toDTO).getResults<COM_NAV>();

            Assert.Equal(1, result!.Distance);
            Assert.Equal(7, result.Direction);
        }
    }
}
