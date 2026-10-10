
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library.eNums;
using static RobsSpacePatrol.DTO.Library.Result;

namespace RobsSpacePatrol.CoreLogic.Tests;

public class DAMTests
{
    [Fact]
    public void test_DAM_ResultNotNull()
    {
        var game = GameBuilder.getGame();

        Assert.NotNull(game.DAM());
    }

    [Fact]
    public void test_DAM_hasContents()
    {
        var game = GameBuilder.getGame();

        var result = game.DAM();

        Assert.NotNull(result.getResults<DAM>());
        Assert.Equal(eCommandResult.OK, result.eCommandResult);
    }

    [Fact]
    public void test_DAM_verifiedContents()
    {
        var rf = new RandomFactoryBuilder()
            .setStarbaseLocation(new Coord(5,1))
            .setStarBaseChance(100)
            .setStarshipQuadrant(new Coord(1,2))
            .setStarshipSector(new Coord(1, 1));

        var game = GameBuilder.getGame(rf);

        game.Galaxy.Starship.setDeviceDamage(eDevice.LRS, -2);
        game.Galaxy.Starship.setDeviceDamage(eDevice.SRS, -1);
        game.Galaxy.Starship.setDeviceDamage(eDevice.TOR, -1.5);

        var result = game.DAM().getResults<DAM>();
        Assert.Equal(3, result!.Devices.Count);




        Assert.Contains(result!.Devices!, z => z.Name == game.Galaxy.Starship.getDevice(eDevice.LRS));
        Assert.Contains(result.Devices, z => z.Name == game.Galaxy.Starship.getDevice(eDevice.SRS));
        Assert.Contains(result.Devices, z => z.Name == game.Galaxy.Starship.getDevice(eDevice.TOR));
        Assert.DoesNotContain(result.Devices, z => z.Name == game.Galaxy.Starship.getDevice(eDevice.NAV));

        Assert.Equal(-1, result.Devices.First(z => z.Name == game.Galaxy.Starship.getDevice(eDevice.SRS)).DamageLevel);
        Assert.Equal(-2, result.Devices.First(z => z.Name == game.Galaxy.Starship.getDevice(eDevice.LRS)).DamageLevel);
        Assert.Equal(-1.5, result.Devices.First(z => z.Name == game.Galaxy.Starship.getDevice(eDevice.TOR)).DamageLevel);
    }
}
