
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library.eNums;
using static RobsSpacePatrol.DTO.Library.Result;

namespace RobsSpacePatrol.CoreLogic.Tests;

public class LRSTests
{
    [Fact]
    public void test_LRS_ResultNotNull()
    {
        var game = GameBuilder.getGame();

        Assert.NotNull(game.LRS());
    }

    [Fact]
    public void test_LRS_hasContents()
    {
        var game = GameBuilder.getGame();

        var result = game.LRS();

        Assert.NotNull(result.getResults<LRS>());
        Assert.Equal(eCommandResult.OK, result.eCommandResult);
    }

    [Fact]
    public void test_LRS_damaged()
    {
        var game = GameBuilder.getGame();

        game.Galaxy.Starship.setDeviceDamage(eDevice.LRS, -3);

        var result = game.LRS();

        Assert.Null(result.getResults<LRS>());
        Assert.Equal(eCommands.LRS, result.Command);
        Assert.Equal(eCommandResult.Damaged, result.eCommandResult);
    }

    [Fact]
    public void test_LRS_verifiedContents()
    {
        var rf = new RandomFactoryBuilder()
            .setStarshipQuadrant(new Coord(1, 1))
            .setStarshipSector(new Coord(0, 0));
        var game = GameBuilder.getGame(rf);

        var result = game.LRS().getResults<LRS>();

        Assert.NotNull(result);
        Assert.Equal(9, result?.Quadrants.Count);
    }

    [Fact]
    public void test_LRS_MarksHasExplored()
    {
        var rf = new RandomFactoryBuilder()
            .setStarshipSector(new Coord(0,0))
            .setStarshipQuadrant(new Coord(3, 3));

        var game = GameBuilder.getGame(out var galaxy, rf);

        var result = game.LRS().getResults<LRS>();

        Assert.Equal(9, result!.Quadrants.Count);
        Assert.True(result.Quadrants.All(z => z.HasBeenExplored));
        Assert.Equal(9, galaxy.Quadrants!.Count(z => z.HasBeenExplored));
    }

    [Fact]
    public void test_LRS_CornerLRSCorrectCountOfQuadrants()
    {
        var rf = new RandomFactoryBuilder()
            .setStarshipSector(new Coord(0,0))
            .setStarshipQuadrant(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);

        var result = game.LRS().getResults<LRS>();

        Assert.Equal(4, result!.Quadrants.Count);
        Assert.True(result.Quadrants.All(z => z.HasBeenExplored));
        Assert.Equal(4, galaxy.Quadrants!.Count(z => z.HasBeenExplored));
    }
}
