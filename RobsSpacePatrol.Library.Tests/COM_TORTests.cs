

using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library;
using RobsSpacePatrol.Library.eNums;
using static RobsSpacePatrol.DTO.Library.Result;

namespace RobsSpacePatrol.CoreLogic.Tests;

public class COM_TORTests
{
    [Fact]
    public void test_COM_TOR_ResultNotNull()
    {
        var game = GameBuilder.getGame();

        Assert.NotNull(game.COM_TOR());
    }

    [Fact]
    public void test_COM_TOR_hasContents()
    {
        var rf = new RandomFactoryBuilder()
                    .addEnemyLocation(new Coord (0, 1))
                    .addEnemyLocation(new Coord (6, 1))
                    .addEnemyLocation(new Coord (4, 4))
                    .setEnemyChance(100)
                    .setStarshipQuadrant(new Coord(1, 2))
                    .setStarshipSector(new Coord(4, 1));

        var game = GameBuilder.getGame(rf);


        var result = game.COM_TOR();

        Assert.NotNull(result.getResults<COM_TOR>());
        Assert.Equal(eCommandResult.OK, result.eCommandResult);
    }

    [Fact]
    public void test_COM_TOR_damaged()
    {
        var rf = new RandomFactoryBuilder()
        .addEnemyLocation(new Coord (0, 1))
        .addEnemyLocation(new Coord (6, 1))
        .addEnemyLocation(new Coord (4, 4))
        .setEnemyChance(0)
        .setStarshipQuadrant(new Coord(1, 2))
        .setStarshipSector(new Coord(4, 1));

        var game = GameBuilder.getGame(rf);

        game.Galaxy.Starship.setDeviceDamage(eDevice.COM, -3);

        var result = game.COM_TOR();

        Assert.Null(result.getResults<COM_TOR>());
        Assert.Equal(eCommands.COM_TOR, result.Command);
        Assert.Equal(eCommandResult.Damaged, result.eCommandResult);
    }

    [Fact]
    public void test_COM_TOR_NoEnemies()
    {
        var rf = new RandomFactoryBuilder()
            .setEnemyChance(0)
            .setStarshipQuadrant(new Coord(1, 2))
            .setStarshipSector(new Coord(4, 1));
        var game = GameBuilder.getGame();

        game.Galaxy.CurrentQuadrant!.Sectors!.ForEach(z => z.SectorContents = Global.SectorEmpty);


        var result = game.COM_TOR();
        var data = result.getResults<COM_TOR>();

        Console.WriteLine(result.eCommandResult.ToString());
        Console.WriteLine(result.Command);
        if (data != null)
        {
            Console.WriteLine(data.DirectionToEnemies.Count);
        }


        Assert.Null(data);
        Assert.Equal(eCommands.COM_TOR, result.Command);
        Assert.Equal(eCommandResult.No_Enemies_Present, result.eCommandResult);
    }

    [Fact]
    public void test_COM_TOR_verifiedContents()
    {
        var rf = new RandomFactoryBuilder()
            .addEnemyLocation(new Coord (0, 1))
            .addEnemyLocation(new Coord (6, 1))
            .addEnemyLocation(new Coord (4, 4))
            .setEnemyChance(100)
            .setStarshipQuadrant(new Coord(1, 2))
            .setStarshipSector(new Coord(4, 1));

        var game = GameBuilder.getGame(rf);

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());
        var result = game.COM_TOR().getResults<COM_TOR>();

        Assert.Equal(3, result!.DirectionToEnemies.Count);

        Assert.NotNull(result.DirectionToEnemies.FirstOrDefault(z => z.Direction == 1));
        Assert.NotNull(result.DirectionToEnemies.FirstOrDefault(z => z.Direction == 7));
        Assert.NotNull(result.DirectionToEnemies.FirstOrDefault(z => z.Direction == 5));

        Assert.NotNull(result.DirectionToEnemies.FirstOrDefault(z => z.Distance == 3));
        Assert.NotNull(result.DirectionToEnemies.FirstOrDefault(z => z.Distance == 2));
        Assert.NotNull(result.DirectionToEnemies.FirstOrDefault(z => z.Distance == 4));
    }
}
