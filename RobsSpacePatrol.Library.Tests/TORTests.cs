
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library.eNums;
using RobsSpacePatrol.Library.Models;
using static RobsSpacePatrol.DTO.Library.Result;

namespace RobsSpacePatrol.CoreLogic.Tests;

public class TORTests
{
    [Fact]
    public void test_TOR_ResultNotNull()
    {
        var game = GameBuilder.getGame();
        game.Galaxy.CurrentQuadrant.ClearQuadrant();

        Assert.NotNull(game.TOR(0));
    }

    [Fact]
    public void test_TOR_damaged()
    {
        var game = GameBuilder.getGame();

        game.Galaxy.Starship.setDeviceDamage(eDevice.TOR, -3);

        var result = game.TOR(0);

        Assert.Null(result.getResults<TOR>());
        Assert.Equal(eCommands.TOR, result.Command);
        Assert.Equal(eCommandResult.Damaged, result.eCommandResult);
    }

    [Fact]
    public void test_TOR_NoTorps()
    {
        var rf = new RandomFactoryBuilder()
           .setStarbaseLocation(new(5, 5))
           .addEnemyLocation(new Coord(1, 0))
           .addEnemyLocation(new Coord(6, 1))
           .addEnemyLocation(new Coord(4, 4))
           .setEnemyChance(100)
           .setStarshipQuadrant(new Coord(0, 0))
           .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);


        galaxy.Starship!.TorpedosRemaining = 0;

        var result = game.TOR(1);

        Assert.Null(result.getResults<TOR>());
        Assert.Equal(eCommands.TOR, result.Command);
        Assert.Equal(eCommandResult.InsufficientInventory, result.eCommandResult);
    }

    [Fact]
    public void test_TOR_verifiedContents_NoEnemies()
    {
        var rf = new RandomFactoryBuilder()
            .setStarbaseLocation(new(5, 5))
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(rf);

        var result = game.TOR(7);
        var data = result.getResults<TOR>();

        Assert.NotNull(result);
        Assert.Null(data);
        Assert.Equal(eCommandResult.No_Enemies_Present, result.eCommandResult);
    }

    [Fact]
    public void test_TOR_verifiedContents_HIT_Enemy()
    {
        var rf = new RandomFactoryBuilder()
            .setStarbaseLocation(new(5, 5))
            .addEnemyLocation(new Coord(1, 0))
            .addEnemyLocation(new Coord(6, 1))
            .addEnemyLocation(new Coord(4, 4))
            .setEnemyChance(100)
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.TOR(1);
        var data = result.getResults<TOR>();

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.True(data.ObjectHitReport.Destroyed);
        Assert.Equal(1, data.ObjectHitReport.Coord!.X);
        Assert.Equal(0, data.ObjectHitReport.Coord.Y);
        Assert.Equal(eObjectHit.Enemy, data.ObjectHitReport.ObjectHit);
        Assert.Equal(Global.maxStarshipTorpedos - 1, galaxy.Starship!.TorpedosRemaining);
    }

    [Fact]
    public void test_TOR_verifiedContents_HIT_One_Enemy()
    {
        var rf = new RandomFactoryBuilder()
            .setStarbaseLocation(new(5, 5))
            .addEnemyLocation(new Coord(1, 0))
            .setEnemyChance(Global.EnemyZeroOne + 2)
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.TOR(1);
        var data = result.getResults<TOR>();

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.True(game.Galaxy.CurrentQuadrant!.Sectors!.All(z => z.Enemy == null));
        Assert.True(data.ObjectHitReport.Destroyed);
        Assert.Equal(1, data.ObjectHitReport.Coord!.X);
        Assert.Equal(0, data.ObjectHitReport.Coord.Y);
        Assert.Equal(eObjectHit.Enemy, data.ObjectHitReport.ObjectHit);
        Assert.Equal(Global.maxStarshipTorpedos - 1, galaxy.Starship!.TorpedosRemaining);
    }

    [Fact]
    public void test_TOR_verifiedContents_MISS()
    {
        var rf = new RandomFactoryBuilder()
            .setStarbaseLocation(new(5, 5))
            .addEnemyLocation(new Coord(1, 0))
            .addEnemyLocation(new Coord(6, 1))
            .addEnemyLocation(new Coord(4, 4))
            .setEnemyChance(100)
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(rf);

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.TOR(7);
        var data = result.getResults<TOR>();

        Assert.NotNull(result);
        Assert.Null(data);
        Assert.Equal(eCommandResult.TOR_Missed, result.eCommandResult);
    }

    [Fact]
    public void test_TOR_verifiedContents_HIT_StarDestroyed()
    {
        var rf = new RandomFactoryBuilder()
            .setStarbaseLocation(new(5, 5))
            .addEnemyLocation(new Coord(1, 7))
            .addEnemyLocation(new Coord(6, 7))
            .addEnemyLocation(new Coord(4, 7))
            .setEnemyChance(100)
            .addStarLocation(new Coord(1, 0))
            .setStarQuantity(1)
            .setStarDestroyedChance(100)
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(rf);

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.TOR(1);
        var data = result.getResults<TOR>();

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.True(data.ObjectHitReport.Destroyed);
        Assert.Equal(1, data.ObjectHitReport.Coord!.X);
        Assert.Equal(0, data.ObjectHitReport.Coord.Y);
        Assert.Equal(eObjectHit.Star, data.ObjectHitReport.ObjectHit);
    }

    [Fact]
    public void test_TOR_verifiedContents_HIT_StarSurvived()
    {
        var rf = new RandomFactoryBuilder()
            .setStarbaseLocation(new(5, 5))
            .addEnemyLocation(new Coord(1, 7))
            .addEnemyLocation(new Coord(6, 7))
            .addEnemyLocation(new Coord(4, 7))
            .setEnemyChance(100)
            .addStarLocation(new Coord(1, 0))
            .setStarQuantity(1)
            .setStarDestroyedChance(0)
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(rf);

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.TOR(1);
        var data = result.getResults<TOR>();

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.False(data.ObjectHitReport.Destroyed);
        Assert.True(data.ObjectHitReport.StarSurvied);
        Assert.Equal(1, data.ObjectHitReport.Coord!.X);
        Assert.Equal(0, data.ObjectHitReport.Coord.Y);
        Assert.Equal(eObjectHit.Star, data.ObjectHitReport.ObjectHit);
    }

    [Fact]
    public void test_TOR_verifiedContents_HIT_StarBase_Destroyed()
    {
        var rf = new RandomFactoryBuilder()
            .addEnemyLocation(new Coord(1, 7))
            .addEnemyLocation(new Coord(6, 7))
            .addEnemyLocation(new Coord(4, 7))
            .setStarbaseLocation(new Coord(1, 0))
            .setStarBaseChance(100)
            .setEnemyChance(100)
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.TOR(1).getResults<TOR>();

        Assert.NotNull(result);
        Assert.True(result.ObjectHitReport.Destroyed);
        Assert.Equal(1, result.ObjectHitReport.Coord!.X);
        Assert.Equal(0, result.ObjectHitReport.Coord.Y);
        Assert.Equal(eObjectHit.Starbase, result.ObjectHitReport.ObjectHit);
        Assert.Equal(Global.maxStarshipTorpedos - 1, galaxy.Starship!.TorpedosRemaining);
    }
}