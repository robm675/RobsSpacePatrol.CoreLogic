
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library.eNums;
using static RobsSpacePatrol.DTO.Library.Result;

namespace RobsSpacePatrol.CoreLogic.Tests;

public class SHETests
{
    [Fact]
    public void test_SHE_ResultNotNull()
    {
        var game = GameBuilder.getGame();

        Assert.NotNull(game.SHE(0));
    }

    [Fact]
    public void test_SHE_hasContents()
    {
        var game = GameBuilder.getGame();

        var result = game.SHE(0);

        Assert.NotNull(result.getResults<SHE>());
        Assert.Equal(eCommandResult.OK, result.eCommandResult);
    }

    [Fact]
    public void test_SHE_damaged()
    {
        var game = GameBuilder.getGame();

        game.Galaxy.Starship.setDeviceDamage(eDevice.SHE, -3);

        var result = game.SHE(0);

        Assert.Null(result.getResults<SHE>());
        Assert.Equal(eCommands.SHE, result.Command);
        Assert.Equal(eCommandResult.Damaged, result.eCommandResult);
    }

    [Fact]
    public void test_SHE_verifiedContents()
    {
        var game = GameBuilder.getGame(out var galaxy);

        int shieldValue = 1000;
        var result = game.SHE(shieldValue).getResults<SHE>();

        Assert.NotNull(result);
        Assert.Equal(shieldValue, galaxy.Starship!.ShieldLevel);
        Assert.Equal(Global.maxStarshipEnergy - shieldValue, galaxy.Starship.EnergyLevel);
        Assert.Equal(string.Empty, result.ErrorMessage);
    }

    [Fact]
    public void test_SHE_badValue()
    {
        var game = GameBuilder.getGame(out var galaxy);

        int shieldValue = 1000;
        int energyValue = 500;

        galaxy.Starship!.EnergyLevel = energyValue;
        var result = game.SHE(shieldValue).getResults<SHE>();

        Assert.NotNull(result);
        Assert.Equal(0, galaxy.Starship.ShieldLevel);
        Assert.Equal(energyValue, galaxy.Starship.EnergyLevel);
        Assert.Equal(Global.SHE_ErrorMessage, result.ErrorMessage);
    }
}
