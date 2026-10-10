using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library.eNums;
using RobsSpacePatrol.Library.Models;
using static RobsSpacePatrol.DTO.Library.Result;

namespace RobsSpacePatrol.CoreLogic.Tests;

public class LASTests
{
    [Fact]
    public void test_LAS_ResultNotNull()
    {
        var game = GameBuilder.getGame();

        Assert.NotNull(game.LAS(0));
    }

    [Fact]
    public void test_LAS_damaged()
    {
        var game = GameBuilder.getGame();

        game.Galaxy.Starship.setDeviceDamage(eDevice.LAS, -3);

        var result = game.LAS(1);

        Assert.Null(result.getResults<LAS>());
        Assert.Equal(eCommands.LAS, result.Command);
        Assert.Equal(eCommandResult.Damaged, result.eCommandResult);
    }

    [Fact]
    public void test_LAS_NoEnergy()
    {
        var rf = new RandomFactoryBuilder()
           .addEnemyLocation(new Coord (1, 0))
           .addEnemyLocation(new Coord (6, 1))
           .addEnemyLocation(new Coord (4, 4))
           .setEnemyChance(100)
           .setStarshipQuadrant(new Coord(0, 0))
           .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);

        galaxy.Starship!.EnergyLevel = 0;

        var result = game.LAS(100);

        Assert.Null(result.getResults<LAS>());
        Assert.Equal(eCommands.LAS, result.Command);
        Assert.Equal(eCommandResult.InsufficientInventory, result.eCommandResult);
    }

    [Fact]
    public void test_LAS_verifiedContents_NoEnemies()
    {
        var rf = new RandomFactoryBuilder()
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(rf);

        var result = game.LAS(7);
        var data = result.getResults<LAS>();

        Assert.NotNull(result);
        Assert.Null(data);
        Assert.Equal(eCommandResult.No_Enemies_Present, result.eCommandResult);
    }

    [Fact]
    public void test_LAS_verifiedContents_HIT_Enemy()
    {
        var rf = new RandomFactoryBuilder()
            .addEnemyLocation(new Coord(1, 0))
            .addEnemyLocation(new Coord(6, 1))
            .addEnemyLocation(new Coord(4, 4))
            .setEnemyChance(100)
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);
        int energyUsed = 1000;

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.LAS(energyUsed);
        var data = result.getResults<LAS>();

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.Equal(3, data.ObjectHitReport.Count);

        var enemy1 = data.ObjectHitReport.FirstOrDefault(z => z.Coord!.X == 1 && z.Coord.Y == 0);
        var enemy2 = data.ObjectHitReport.FirstOrDefault(z => z.Coord!.X == 6 && z.Coord.Y == 1);
        var enemy3 = data.ObjectHitReport.FirstOrDefault(z => z.Coord!.X == 4 && z.Coord.Y == 4);

        //var enemy1 = data.ObjectHitReport.FirstOrDefault(z => comp.Compare(z.Coord, new Coord { X = 1, Y=0 }).Equal);
        //var enemy2 = data.ObjectHitReport.FirstOrDefault(z => comp.Compare(z.Coord, new Coord { X = 6, Y=1 }).Equal);
        //var enemy3 = data.ObjectHitReport.FirstOrDefault(z => comp.Compare(z.Coord, new Coord { X = 4, Y=4 }).Equal);

        Assert.NotNull(enemy1);
        Assert.NotNull(enemy2);
        Assert.NotNull(enemy3);

        Assert.False(enemy1.Missed);
        Assert.False(enemy2.Missed);
        Assert.False(enemy3.Missed);

        Assert.Equal(eObjectHit.Enemy, enemy1.ObjectHit);
        Assert.Equal(eObjectHit.Enemy, enemy2.ObjectHit);
        Assert.Equal(eObjectHit.Enemy, enemy3.ObjectHit);

        Assert.NotEqual(Global.maxStarshipEnergy, galaxy.Starship!.EnergyLevel);
        Assert.Equal(Global.maxStarshipEnergy - energyUsed, galaxy.Starship.EnergyLevel);
    }

    [Fact]
    public void test_LAS_verifiedContents_MISS()
    {
        var rf = new RandomFactoryBuilder()
            .addEnemyLocation(new Coord(1,0))
            .addEnemyLocation(new Coord (6, 1))
            .addEnemyLocation(new Coord (4, 4))
            .setEnemyChance(100)
            .setLaserMissChance(100)
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);
        int energyUsed = 1000;

        game.Galaxy.Starship.setDeviceDamage(eDevice.COM, -3);

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.LAS(energyUsed);
        var data = result.getResults<LAS>();

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.Equal(3, data.ObjectHitReport.Count);

        var enemy1 = data.ObjectHitReport.FirstOrDefault(z => z.Coord!.X == 1 && z.Coord.Y == 0);
        var enemy2 = data.ObjectHitReport.FirstOrDefault(z => z.Coord!.X == 6 && z.Coord.Y == 1);
        var enemy3 = data.ObjectHitReport.FirstOrDefault(z => z.Coord!.X == 4 && z.Coord.Y == 4);

        Assert.NotNull(enemy1);
        Assert.NotNull(enemy2);
        Assert.NotNull(enemy3);

        Assert.True(enemy1.Missed);
        Assert.True(enemy2.Missed);
        Assert.True(enemy3.Missed);

        Assert.Equal(eObjectHit.Enemy, enemy1.ObjectHit);
        Assert.Equal(eObjectHit.Enemy, enemy2.ObjectHit);
        Assert.Equal(eObjectHit.Enemy, enemy3.ObjectHit);

        Assert.NotEqual(Global.maxStarshipEnergy, galaxy.Starship!.EnergyLevel);
        Assert.Equal(Global.maxStarshipEnergy - energyUsed, galaxy.Starship.EnergyLevel);
    }
}
