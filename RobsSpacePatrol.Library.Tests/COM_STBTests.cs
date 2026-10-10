
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library.eNums;
using static RobsSpacePatrol.DTO.Library.Result;

namespace RobsSpacePatrol.CoreLogic.Tests
{
    public class COM_STBTests
    {
        [Fact]
        public void test_COM_STB_ResultNotNull()
        {
            var rf = new RandomFactoryBuilder()
              .setStarbaseLocation(new Coord (5, 1))
              .setStarBaseChance(100)
              .setStarshipQuadrant(new Coord(1, 2))
              .setStarshipSector(new Coord(1, 1));

            var game = GameBuilder.getGame(rf);

            Assert.NotNull(game.COM_STB());
        }

        [Fact]
        public void test_COM_STB_hasContents()
        {
            var rf = new RandomFactoryBuilder()
                .setStarbaseLocation(new Coord (5, 1))
                .setStarBaseChance(100)
                .setStarshipQuadrant(new Coord(1, 2))
                .setStarshipSector(new Coord(1, 1));

            var game = GameBuilder.getGame(rf);

            var result = game.COM_STB();

            Assert.NotNull(result.getResults<COM_STB>());
            Assert.Equal(eCommandResult.OK, result.eCommandResult);
        }

        [Fact]
        public void test_COM_STB_damaged()
        {
            var game = GameBuilder.getGame();

            game.Galaxy.Starship.setDeviceDamage(eDevice.COM, -3);

            var result = game.COM_STB();

            Assert.Null(result.getResults<COM_STB>());
            Assert.Equal(eCommands.COM_STB, result.Command);
            Assert.Equal(eCommandResult.Damaged, result.eCommandResult);
        }

        [Fact]
        public void test_COM_STB_verifiedContents()
        {
            var rf = new RandomFactoryBuilder()
                .setStarbaseLocation(new Coord (5, 1))
                .setStarBaseChance(100)
                .setStarshipQuadrant(new Coord(1, 2))
                .setStarshipSector(new Coord(1, 1));

            var game = GameBuilder.getGame(rf);

            var result = game.COM_STB().getResults<COM_STB>();

            Assert.Equal(4, result!.Distance);
            Assert.Equal(1, result.Direction);
            Assert.Equal(5, result.StarBaseCoord!.X);
            Assert.Equal(1, result.StarBaseCoord.Y);
        }

        [Fact]
        public void test_COM_STB_NoStarBase()
        {
            var rf = new RandomFactoryBuilder()
                .setStarshipQuadrant(new Coord(1, 2))
                .setStarshipSector(new Coord(1, 1));

            var game = GameBuilder.getGame(rf);

            var result = game.COM_STB();
            var data = result.getResults<COM_STB>();

            Assert.NotNull(result);
            Assert.Null(data);
            Assert.Equal(eCommandResult.CPU_STB_No_Starbase_Present, result.eCommandResult);
        }
    }
}
