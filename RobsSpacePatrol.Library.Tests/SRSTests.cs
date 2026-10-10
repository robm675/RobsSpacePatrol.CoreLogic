

using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library.eNums;
using static RobsSpacePatrol.DTO.Library.Result;

namespace RobsSpacePatrol.CoreLogic.Tests;

public class SRSTests
{
    [Fact]
    public void test_SRS_ResultNotNull()
    {
        var game = GameBuilder.getGame();

        Assert.NotNull(game.SRS());
    }

    [Fact]
    public void test_SRS_hasContents()
    {
        var game = GameBuilder.getGame();

        var result = game.SRS();

        Assert.NotNull(result.getResults<SRS>());
        Assert.Equal(eCommandResult.OK, result.eCommandResult);
    }

    [Fact]
    public void test_SRS_damaged()
    {
        var game = GameBuilder.getGame();
        game.Galaxy.Starship.setDeviceDamage(eDevice.SRS, -3);

        var result = game.SRS();

        Assert.Null(result.getResults<SRS>());
        Assert.Equal(eCommands.SRS, result.Command);
        Assert.Equal(eCommandResult.Damaged, result.eCommandResult);
    }

    [Fact]
    public void test_SRS_verifiedContents()
    {
        var game = GameBuilder.getGame();

        var result = game.SRS().getResults<SRS>();

        Assert.NotNull(result);
        Assert.Equal(Global.MaxSectorsXY * Global.MaxSectorsXY, result?.Sectors.Count);
    }
}
