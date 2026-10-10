using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library.eNums;
using static RobsSpacePatrol.DTO.Library.Result;

[assembly: CaptureConsole]

namespace RobsSpacePatrol.CoreLogic.Tests;

public class NAVTests
{
    private readonly ITestOutputHelper output;

    public NAVTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void basicTest()
    {
        var game = GameBuilder.getGame();
        game.Galaxy.CurrentQuadrant.ClearQuadrant();

        var result = game.NAV(1, 1);

        Assert.NotNull(result);
    }

    [Fact]
    public void testBadDIR_0()
    {
        var game = GameBuilder.getGame();

        var result = game.NAV(0, 1);
        var data = result.getResults<NAV>();

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.Equal(eCommandResult.Error, result.eCommandResult);
        Assert.Equal(eNavMessage.InvalidDIR, data.NavMessage);
    }

    [Fact]
    public void testBadDIR_9()
    {
        var game = GameBuilder.getGame();

        var result = game.NAV(9, 1);
        var data = result.getResults<NAV>();

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.Equal(eCommandResult.Error, result.eCommandResult);
        Assert.Equal(eNavMessage.InvalidDIR, data.NavMessage);
    }

    [Fact]
    public void testBadDIST_LT_Point1()
    {
        var game = GameBuilder.getGame();

        var result = game.NAV(1, 0);
        var data = result.getResults<NAV>();

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.Equal(eCommandResult.Error, result.eCommandResult);
        Assert.Equal(eNavMessage.InvalidDIST, data.NavMessage);
    }

    [Fact]
    public void testBadDIST_GT_8()
    {
        var game = GameBuilder.getGame();

        var result = game.NAV(1, 9);
        var data = result.getResults<NAV>();

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.Equal(eCommandResult.Error, result.eCommandResult);
        Assert.Equal(eNavMessage.InvalidDIST, data.NavMessage);
    }

    [Fact]
    public void testDamaged_InvalidDist()
    {
        var game = GameBuilder.getGame();

        game.Galaxy.Starship.setDeviceDamage(eDevice.NAV, -3);

        var result = game.NAV(1, 2);
        var data = result.getResults<NAV>();

        Assert.NotNull(result);
        Assert.Null(data);
        Assert.Equal(eCommandResult.Damaged, result.eCommandResult);
    }

    [Fact]
    public void testDamaged_ValidDist()
    {
        var game = GameBuilder.getGame();
        game.Galaxy.CurrentQuadrant.ClearQuadrant();

        game.Galaxy.Starship.setDeviceDamage(eDevice.NAV, -3);

        output.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.NAV(1, .2);
        var data = result.getResults<NAV>();

        output.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.Equal(eCommandResult.OK, result.eCommandResult);
        Assert.Equal(eNavMessage.TransitComplete, data.NavMessage);
    }

    [Fact]
    public void testDamaged_OutsideGalaxyTop()
    {
        var rf = new RandomFactoryBuilder()
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(rf);

        var result = game.NAV(3, 1);
        var data = result.getResults<NAV>();

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.Equal(eCommandResult.Error, result.eCommandResult);
        Assert.Equal(eNavMessage.BadInput_OutsideGalaxy, data.NavMessage);
    }

    [Fact]
    public void testDamaged_OutsideGalaxyLeft()
    {
        var rf = new RandomFactoryBuilder()
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(rf);

        var result = game.NAV(5, 1);
        var data = result.getResults<NAV>();

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.Equal(eCommandResult.Error, result.eCommandResult);
        Assert.Equal(eNavMessage.BadInput_OutsideGalaxy, data.NavMessage);
    }

    [Fact]
    public void testDamaged_OutsideGalaxyBottom()
    {
        var rf = new RandomFactoryBuilder()
            .setStarshipQuadrant(new Coord(7, 7))
            .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(rf);

        var result = game.NAV(7, 1);
        var data = result.getResults<NAV>();

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.Equal(eCommandResult.Error, result.eCommandResult);
        Assert.Equal(eNavMessage.BadInput_OutsideGalaxy, data.NavMessage);
    }

    [Fact]
    public void testDamaged_OutsideGalaxyRight()
    {
        var rf = new RandomFactoryBuilder()
            .setStarshipQuadrant(new Coord(7, 7))
            .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(rf);

        var result = game.NAV(1, 1);
        var data = result.getResults<NAV>();

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.Equal(eCommandResult.Error, result.eCommandResult);
        Assert.Equal(eNavMessage.BadInput_OutsideGalaxy, data.NavMessage);
    }

    [Fact]
    public void testDamaged_NotEnoughEnergy_ShieldAvail()
    {
        var rf = new RandomFactoryBuilder()
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);

        galaxy.Starship!.EnergyLevel = 10;
        galaxy.Starship.ShieldLevel = 1000;

        var result = game.NAV(1, 7);
        var data = result.getResults<NAV>();

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.Equal(eCommandResult.Error, result.eCommandResult);
        Assert.Equal(eNavMessage.InsufficientEnergy_ShieldEnergyAvailable, data.NavMessage);
    }

    [Fact]
    public void testDamaged_NotEnoughEnergy()
    {
        var rf = new RandomFactoryBuilder()
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);

        galaxy.Starship!.EnergyLevel = 10;
        galaxy.Starship.ShieldLevel = 0;

        var result = game.NAV(1, 7);
        var data = result.getResults<NAV>();

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.Equal(eCommandResult.Error, result.eCommandResult);
        Assert.Equal(eNavMessage.InsufficientEnergy, data.NavMessage);
    }

    [Fact]
    public void testDamaged_ObjectInWay()
    {
        var rf = new RandomFactoryBuilder()
            .setStarQuantity(1)
            .addStarLocation(new Coord (5,0))
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.NAV(1, 7);
        var data = result.getResults<NAV>();

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.Equal(eCommandResult.Error, result.eCommandResult);
        Assert.Equal(eNavMessage.BadInput_ObjectHit, data.NavMessage);
        Assert.NotEqual(0, data.DistanceTraveled);
    }

    [Fact]
    public void testDamaged_Good_InsideQuadrant()
    {
        var rf = new RandomFactoryBuilder()
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);

        output.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.NAV(1, .1);
        var data = result.getResults<NAV>();

        output.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.Equal(eCommandResult.OK, result.eCommandResult);
        Assert.Equal(eNavMessage.TransitComplete, data.NavMessage);
        Assert.NotEqual(0, data.DistanceTraveled);
        Assert.NotEqual(Global.maxStarshipEnergy, galaxy.Starship!.EnergyLevel);
    }

    [Fact]
    public void testDamaged_Good_DifferentQuadrant()
    {
        var rf = new RandomFactoryBuilder()
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);

        output.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.NAV(1, 7);
        var data = result.getResults<NAV>();

        Assert.NotNull(result);
        Assert.NotNull(data);
        Assert.Equal(eCommandResult.OK, result.eCommandResult);
        Assert.Equal(eNavMessage.TransitComplete, data.NavMessage);
        Assert.NotEqual(0, data.DistanceTraveled);
        Assert.NotEqual(Global.maxStarshipEnergy, galaxy.Starship!.EnergyLevel);
    }

    [Fact]
    public void test_CollisionTest()
    {
        var rf = new RandomFactoryBuilder()
          .setStarBaseChance(100)
          .setStarbaseLocation(new Coord(0, 3))
          .setStarshipQuadrant(new Coord(0, 0))
          .setStarshipSector(new Coord(0, 5));

        var game = GameBuilder.getGame(out var galaxy, rf);

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.NAV(3, .5);
        var data = result.getResults<NAV>();

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        Assert.Equal(0, galaxy.CurrentQuadrant.GetSectorByContents(Global.SectorStarship)!.Coord.X);
        Assert.Equal(4, galaxy.CurrentQuadrant.GetSectorByContents(Global.SectorStarship)!.Coord.Y);
    }

    [Fact]
    public void test_VerifyStarshipSector()
    {
        var rf = new RandomFactoryBuilder()
          .setStarBaseChance(100)
          .setStarbaseLocation(new Coord(0, 3))
          .setStarshipQuadrant(new Coord(0, 0))
          .setStarshipSector(new Coord(0, 5));

        var game = GameBuilder.getGame(out var galaxy, rf);

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.NAV(1, .5);
        var data = result.getResults<NAV>();

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        Assert.Equal(4, galaxy.CurrentQuadrant.GetSectorByContents(Global.SectorStarship)!.Coord.X);
        Assert.Equal(5, galaxy.CurrentQuadrant.GetSectorByContents(Global.SectorStarship)!.Coord.Y);
    }

    [Fact]
    public void test_VerifyLocationAfterNAV_Space1()
    {
        var rf = new RandomFactoryBuilder()
          .setStarshipQuadrant(new Coord(0, 0))
          .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());
        Console.WriteLine(game.Galaxy.getGalaxyFormatted());


        var result = game.NAV(1, 1);
        var data = result.getResults<NAV>();


        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());
        Console.WriteLine(game.Galaxy.getGalaxyFormatted());

        Assert.Equal(1, galaxy.CurrentQuadrant!.Coord!.X);
        Assert.Equal(0, galaxy.CurrentQuadrant.Coord.Y);
    }

    [Fact]
    public void test_VerifyLocationAfterNAV_Space1_Point1()
    {
        var rf = new RandomFactoryBuilder()
          .setStarshipQuadrant(new Coord(0, 0))
          .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());
        Console.WriteLine(game.Galaxy.getGalaxyFormatted());


        var result = game.NAV(1, 1.1);
        var data = result.getResults<NAV>();


        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());
        Console.WriteLine(game.Galaxy.getGalaxyFormatted());

        Assert.Equal(1, galaxy.CurrentQuadrant!.Coord!.X);
        Assert.Equal(0, galaxy.CurrentQuadrant.Coord.Y);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void NAV_RejectsNonFiniteDirection(double direction)
    {
        var game = GameBuilder.getGame();

        var result = game.NAV(direction, 1);
        var data = result.getResults<Result.NAV>();

        Assert.Equal(eCommandResult.Error, result.eCommandResult);
        Assert.NotNull(data);
        Assert.Equal(eNavMessage.InvalidDIR, data.NavMessage);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void NAV_RejectsNonFiniteDistance(double distance)
    {
        var game = GameBuilder.getGame();

        var result = game.NAV(1, distance);
        var data = result.getResults<Result.NAV>();

        Assert.Equal(eCommandResult.Error, result.eCommandResult);
        Assert.NotNull(data);
        Assert.Equal(eNavMessage.InvalidDIST, data.NavMessage);
    }
}
