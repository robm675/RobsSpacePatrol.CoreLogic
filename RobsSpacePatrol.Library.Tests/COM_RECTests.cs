
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Factories;
using RobsSpacePatrol.Library.eNums;
using static RobsSpacePatrol.DTO.Library.Result;

namespace RobsSpacePatrol.CoreLogic.Tests
{
    public class COM_RECTests
    {
        [Fact]
        public void test_COM_REC_ResultNotNull()
        {
            var galaxy = new GalaxyFactory().createGalaxy(new RandomFactory(), new CurrentQuadrantFactory(), new QuadrantFactory());
            var game = new Game(galaxy, new RandomFactory(), new CurrentQuadrantFactory());

            Assert.NotNull(game.COM_REC());
        }

        [Fact]
        public void test_COM_REC_hasContents()
        {
            var galaxy = new GalaxyFactory().createGalaxy(new RandomFactory(), new CurrentQuadrantFactory(), new QuadrantFactory());
            var game = new Game(galaxy, new RandomFactory(), new CurrentQuadrantFactory());

            var result = game.COM_REC();

            Assert.NotNull(result.getResults<COM_REC>());
            Assert.Equal(eCommandResult.OK, result.eCommandResult);
        }

        [Fact]
        public void test_COM_REC_damaged()
        {
            var galaxy = new GalaxyFactory().createGalaxy(new RandomFactory(), new CurrentQuadrantFactory(), new QuadrantFactory());
            var game = new Game(galaxy, new RandomFactory(), new CurrentQuadrantFactory());

            game.Galaxy.Starship.setDeviceDamage(eDevice.COM, -3);

            var result = game.COM_REC();

            Assert.Null(result.getResults<COM_REC>());
            Assert.Equal(eCommands.COM_REC, result.Command);
            Assert.Equal(eCommandResult.Damaged, result.eCommandResult);
        }

        [Fact]
        public void test_COM_REC_verifiedContents()
        {
            var galaxy = new GalaxyFactory().createGalaxy(new RandomFactory(), new CurrentQuadrantFactory(), new QuadrantFactory());
            var game = new Game(galaxy, new RandomFactory(), new CurrentQuadrantFactory());

            var result = game.COM_REC().getResults<COM_REC>();

            Assert.NotNull(result);
            Assert.NotEqual(0, result?.Quadrants.Count);
        }
    }
}
