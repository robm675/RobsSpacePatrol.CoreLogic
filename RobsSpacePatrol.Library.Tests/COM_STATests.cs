
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Factories;
using RobsSpacePatrol.Library;
using RobsSpacePatrol.Library.eNums;
using static RobsSpacePatrol.DTO.Library.Result;

namespace RobsSpacePatrol.CoreLogic.Tests
{
    public class COM_STATests
    {
        [Fact]
        public void test_COM_STA_ResultNotNull()
        {
            var galaxy = new GalaxyFactory().createGalaxy(new RandomFactory(), new CurrentQuadrantFactory(), new QuadrantFactory());
            var game = new Game(galaxy, new RandomFactory(), new CurrentQuadrantFactory());

            Assert.NotNull(game.COM_STA());
        }

        [Fact]
        public void test_COM_STA_hasContents()
        {
            var galaxy = new GalaxyFactory().createGalaxy(new RandomFactory(), new CurrentQuadrantFactory(), new QuadrantFactory());
            var game = new Game(galaxy, new RandomFactory(), new CurrentQuadrantFactory());

            var result = game.COM_STA();

            Assert.NotNull(result.getResults<COM_STA>());
            Assert.Equal(eCommandResult.OK, result.eCommandResult);
        }

        [Fact]
        public void test_COM_STA_damaged()
        {
            var galaxy = new GalaxyFactory().createGalaxy(new RandomFactory(), new CurrentQuadrantFactory(), new QuadrantFactory());
            var game = new Game(galaxy, new RandomFactory(), new CurrentQuadrantFactory());

            game.Galaxy.Starship.setDeviceDamage(eDevice.COM, -3);

            var result = game.COM_STA();

            Assert.Null(result.getResults<COM_STA>());
            Assert.Equal(eCommands.COM_STA, result.Command);
            Assert.Equal(eCommandResult.Damaged, result.eCommandResult);
        }

        [Fact]
        public void test_COM_STA_verifiedContents()
        {
            var rf = new RandomFactoryBuilder()
                .addEnemyLocation(new Coord(1,2))
                .addEnemyLocation(new Coord (1, 3))
                .addEnemyLocation(new Coord (1, 4))
                .setStarbaseLocation(new Coord (1, 5))
                .setStartingMissionTime(1234)
                .setEnemyChance(100)
                .setStarBaseChance(100)
                .setStarshipQuadrant(new Coord (1,2))
                .setStarshipSector(new Coord(3, 4));

            var galaxy = new GalaxyFactory().createGalaxy(rf.GetFactories(), new CurrentQuadrantFactory(), new QuadrantFactory());
            galaxy.Starship!.ShieldLevel = 5;
            galaxy.Starship.TorpedosRemaining = 6;
            galaxy.Starship.EnergyLevel = 7;
            galaxy.Starship.isDocked = true;

            var enemiesRemaining = Global.MaxQuadrantsXY * Global.MaxQuadrantsXY * 3;
            var sddl = galaxy.MissionTime + (enemiesRemaining * Global.MissionTimeUnitsPerEnemyRatio);

            var game = new Game(galaxy, new RandomFactory(), new CurrentQuadrantFactory());
            var result = game.COM_STA().getResults<COM_STA>();

            Assert.NotNull(result);
            Assert.Equal(5, result.ShieldLevel);
            Assert.Equal(6, result.TorpedosRemaining);
            Assert.Equal(7, result.EnergyRemaining);
            Assert.True(result.Docked);
            Assert.Equal(enemiesRemaining, result.EnemiesRemaining);
            Assert.Equal(Global.MaxQuadrantsXY * Global.MaxQuadrantsXY, result.StarBaseRemaining);
            Assert.Equal(1, result.QuadrantCoord.X);
            Assert.Equal(2, result.QuadrantCoord.Y);
            Assert.Equal(3, result.SectorCoord.X);
            Assert.Equal(4, result.SectorCoord.Y);
            Assert.Equal(1234, result.MissionTime);
            Assert.Equal(sddl, result.MissionTimeDeadLine);
        }
    }
}
