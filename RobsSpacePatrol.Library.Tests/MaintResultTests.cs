
using Microsoft.VisualStudio.TestPlatform.Utilities;
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library.eNums;
using static RobsSpacePatrol.DTO.Library.Result;

namespace RobsSpacePatrol.CoreLogic.Tests;

public class MaintResultTests
{
    private readonly ITestOutputHelper output;

    public MaintResultTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    [InlineData(0, 1)]
    [InlineData(2, 1)]
    [InlineData(0, 2)]
    [InlineData(1, 2)]
    [InlineData(2, 2)]
    public void verify_DockingWorks(int sectorX, int sectorY)
    {
        var rf = new RandomFactoryBuilder()
           .setStarBaseChance(100)
           .setStarbaseLocation(new Coord(1, 1))
           .setStarshipQuadrant(new Coord(0, 0))
           .setStarshipSector(new Coord(sectorX, sectorY));

        var game = GameBuilder.getGame(out var galaxy, rf);

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.RunRoutineMaintenance(0);
        var data = result.getResults<MaintResult>();


        Assert.True(data!.DockingStatus!.CurrentStatus);
    }

    [Fact]
    public void verify_NotDockedShieldsUp()
    {
        var rf = new RandomFactoryBuilder()
           .setStarBaseChance(100)
           .setStarbaseLocation(new Coord (1, 1))
           .setStarshipQuadrant(new Coord (0, 0))
           .setStarshipSector(new Coord (0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);

        galaxy.Starship!.ShieldLevel = 1000;

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.RunRoutineMaintenance(0);
        var data = result.getResults<MaintResult>();


        Assert.False(data!.DockingStatus!.CurrentStatus);
    }

    [Theory]
    [InlineData(6, 1)]
    [InlineData(7, 1)]
    [InlineData(7, 4)]
    [InlineData(4, 3)]
    public void Verify_NotDockedWorks(int sectorX, int sectorY)
    {
        var rf = new RandomFactoryBuilder()
           .setStarBaseChance(100)
           .setStarbaseLocation(new Coord(1,1))
           .setStarshipQuadrant(new Coord (0, 0))
           .setStarshipSector(new Coord (sectorX, sectorY));

        var game = GameBuilder.getGame(out var galaxy, rf);

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.RunRoutineMaintenance(0);
        var data = result.getResults<MaintResult>();


        Assert.False(data!.DockingStatus!.CurrentStatus);
    }

    [Fact]
    public void verify_UpdateMissionTime()
    {
        int startMissionTime = 2400;
        var rf = new RandomFactoryBuilder()
         .setStarBaseChance(100)
         .setStartingMissionTime(startMissionTime)
         .setStarbaseLocation(new Coord (1, 1))
         .setStarshipQuadrant(new Coord (0, 0))
         .setStarshipSector(new Coord (0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);
        var result = game.RunRoutineMaintenance(1);

        Assert.NotEqual(startMissionTime, galaxy.MissionTime);
    }

    [Fact]
    public void verify_NoRepairsAvailable()
    {
        var rf = new RandomFactoryBuilder()
        .setStarBaseChance(100)
        .setStarbaseLocation(new Coord(1,1))
        .setStarshipQuadrant(new Coord (0, 0))
        .setStarshipSector(new Coord (0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.RunRoutineMaintenance(0);
        var data = result.getResults<MaintResult>();

        Assert.True(data!.DockingStatus!.CurrentStatus);
        Assert.Null(data.StarbaseRepairsMissionTimeUnits);
    }

    [Fact]
    public void verify_RepairsAvailable()
    {
        var rf = new RandomFactoryBuilder()
        .setStarBaseChance(100)
        .setStarbaseLocation(new Coord (1, 1))
        .setStarshipQuadrant(new Coord (0, 0))
        .setStarshipSector(new Coord (0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);

        game.Galaxy.Starship.setDeviceDamage(eDevice.LRS, -3);

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.RunRoutineMaintenance(0);
        var data = result.getResults<MaintResult>();

        Assert.True(data!.DockingStatus!.CurrentStatus);
        Assert.NotNull(data.StarbaseRepairsMissionTimeUnits);
    }

    [Fact]
    public void verify_RepairsAvailableCorrectMissionTimeAmount()
    {
        int damageAmount = -3;
        var rf = new RandomFactoryBuilder()
        .setStarBaseChance(100)
        .setStarbaseLocation(new Coord (1,1))
        .setStarshipQuadrant(new Coord (0, 0))
        .setStarshipSector(new Coord (0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);

        game.Galaxy.Starship.setDeviceDamage(eDevice.LRS, damageAmount);

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.RunRoutineMaintenance(0);
        var data = result.getResults<MaintResult>();

        Assert.True(data!.DockingStatus!.CurrentStatus);
        Assert.NotNull(data.StarbaseRepairsMissionTimeUnits);
        Assert.Equal(damageAmount, data.StarbaseRepairsMissionTimeUnits);
    }

    //[Fact]
    //public void verify_repairsComplete()
    //{
    //    int damageAmount = -3;
    //    int startingMissionTime = 2400;
    //    var rf = new RandomFactoryBuilder()
    //        .setStartingMissionTime(startingMissionTime)
    //        .setStarBaseChance(100)
    //        .setStarbaseLocation(new Coord (1, 1))
    //        .setStarshipQuadrant(new Coord (0, 0))
    //        .setStarshipSector(new Coord (0, 0));

    //    var game = GameBuilder.getGame(out var galaxy, rf);

    //    game.Galaxy.Starship.setDeviceDamage(eDevice.LRS, damageAmount);

    //    Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

    //    var result = game.RunRoutineMaintenance(0, false);
    //    var data = result.getResults<MaintResult>();

    //    Assert.True(data!.DockingStatus!.CurrentStatus);
    //    Assert.NotNull(data.StarbaseRepairsMissionTimeUnits);
    //    Assert.Equal(damageAmount, data.StarbaseRepairsMissionTimeUnits);

    //    data.StarbaseRepairsAvailable.StarbaseMakeRepairs(damageAmount);

    //    Assert.Equal(0, game.Galaxy.Starship.getDeviceDamage(eDevice.LRS));
    //    Assert.NotEqual(startingMissionTime, galaxy.MissionTime);
    //}

    [Fact]
    public void verify_DamagedDevicesRepairOnMissionTime()
    {
        int damageAmount = -3;
        int routineMissionTimeChange = 1;
        var rf = new RandomFactoryBuilder()
          .setStarBaseChance(100)
          .setStarbaseLocation(new Coord(1,1))
          .setStarshipQuadrant(new Coord (0, 0))
          .setStarshipSector(new Coord (0, 0) );

        var game = GameBuilder.getGame(out var galaxy, rf);

        game.Galaxy.Starship.setDeviceDamage(eDevice.LRS, damageAmount);

        Console.WriteLine(game.Galaxy.CurrentQuadrant.getFormatted());

        var result = game.RunRoutineMaintenance(routineMissionTimeChange);
        var data = result.getResults<MaintResult>();

        Assert.True(data!.DockingStatus!.CurrentStatus);
        Assert.NotNull(data.StarbaseRepairsMissionTimeUnits);

        var drsc = data!.DevicesRepairStatusChanged!.First(z => z.Name == game.Galaxy.Starship.getDevice(eDevice.LRS));
        Assert.Equal(damageAmount + routineMissionTimeChange, drsc.DamageLevel);
    }

    [Fact]
    public void verify_EnemiesFired()
    {
        var rf = new RandomFactoryBuilder()
           .addEnemyLocation(new Coord (1, 1))
           .addEnemyLocation(new Coord (6, 6))
           .addEnemyLocation(new Coord (4, 4))
           .setEnemyChance(100)
           .setStarBaseChance(100)
           .setEnemyMoveChance(0)
           .setStarQuantity(0)
           .setStarbaseLocation(new Coord (1, 5))
           .setStarshipQuadrant(new Coord (0, 0))
           .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);
        game.Galaxy.debugSet(skipEnemyMove: true);
        var result = game.RunRoutineMaintenance(1);
        var data = result.getResults<MaintResult>();

        Assert.NotNull(data!.EnemiesFired);
        Assert.Equal(3, data.EnemiesFired.Count);
    }

    [Fact]
    public void verify_EnemiesFired_DevicesDamaged()
    {
        int damagedDeviceAmount = -2;
        var rf = new RandomFactoryBuilder()
           .addEnemyLocation(new Coord(1,0))
           .addEnemyLocation(new Coord(6,1))
           .addEnemyLocation(new Coord (4, 4))
           .setEnemyChance(100)
           .setStarBaseChance(0)
           .setEnemyMoveChance(0)
           .setDamagedDevice(3)
           .setDamagedDeviceChance(100)
           .setDamagedDeviceAmount(damagedDeviceAmount)
           .setStarbaseLocation(new Coord (1, 1))
           .setStarshipQuadrant(new Coord(0, 0))
           .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);
        var result = game.RunRoutineMaintenance(1);
        var data = result.getResults<MaintResult>();

        var enemy1 = data!.EnemiesFired![0];
        var enemy2 = data.EnemiesFired[1];
        var enemy3 = data.EnemiesFired[2];

        Assert.NotNull(enemy1.DeviceDamaged);
        Assert.NotNull(enemy2.DeviceDamaged);
        Assert.NotNull(enemy3.DeviceDamaged);

        Assert.Equal(Math.Abs(damagedDeviceAmount), enemy1.DeviceDamangedAmount);
        Assert.Equal(Math.Abs(damagedDeviceAmount), enemy2.DeviceDamangedAmount);
        Assert.Equal(Math.Abs(damagedDeviceAmount), enemy3.DeviceDamangedAmount);
    }

    [Fact]
    public void verify_EnemiesFiredWhileDocked()
    {
        var rf = new RandomFactoryBuilder()
           .addEnemyLocation(new Coord(1,0))
           .addEnemyLocation(new Coord(6,1))
           .addEnemyLocation(new Coord (4, 4))
           .setEnemyChance(100)
           .setStarBaseChance(100)
           .setEnemyMoveChance(0)
           .setStarbaseLocation(new Coord (1, 1))
           .setStarshipQuadrant(new Coord(0, 0))
           .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);

        var result = game.RunRoutineMaintenance(1);
        var data = result.getResults<MaintResult>();


        var enemy1 = data!.EnemiesFired![0];
        var enemy2 = data.EnemiesFired[1];
        var enemy3 = data.EnemiesFired[2];

        Assert.True(enemy1.StarshipDocked);
        Assert.True(enemy2.StarshipDocked);
        Assert.True(enemy3.StarshipDocked);
    }

    [Fact]
    public void verify_GameStatus_OutofTime()
    {
        var rf = new RandomFactoryBuilder()
           .addEnemyLocation(new Coord(1,0))
           .addEnemyLocation(new Coord (6, 1))
           .addEnemyLocation(new Coord (4, 4))
           .setEnemyChance(100)
           .setStarBaseChance(100)
           .setStarbaseLocation(new Coord (1, 1))
           .setStarshipQuadrant(new Coord(0, 0))
           .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);
        galaxy.MissionTime = galaxy.MissionTimeDeadLine + 1;

        var result = game.RunRoutineMaintenance(1);
        var data = result.getResults<MaintResult>();

        Assert.Equal(eGameStatus.RanOutOfTime, data!.GameStatus);
    }

    [Fact]
    public void verify_GameStatus_StarshipIsDestroyed()
    {
        var rf = new RandomFactoryBuilder()
           .addEnemyLocation(new Coord(1,0))
           .addEnemyLocation(new Coord (6, 1))
           .addEnemyLocation(new Coord (4, 4))
           .setEnemyChance(100)
           .setEnemyMoveChance(0)
           .setStarBaseChance(100)
           .setStarbaseLocation(new Coord (1, 1))
           .setStarshipQuadrant(new Coord(0, 0))
           .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);
        galaxy.Starship!.isDestroyed = true;

        var result = game.RunRoutineMaintenance(1);
        var data = result.getResults<MaintResult>();

        Assert.Equal(eGameStatus.StarshipDestroyed, data!.GameStatus);
    }

    [Fact]
    public void verify_GameStatus_NoEnergyNoShields()
    {
        var rf = new RandomFactoryBuilder()
           .addEnemyLocation(new Coord (1, 0))
           .addEnemyLocation(new Coord (6, 1))
           .addEnemyLocation(new Coord (4, 4))
           .setEnemyMoveChance(0)
           .setEnemyChance(100)
           .setStarbaseLocation(new Coord (1, 1))
           .setStarshipQuadrant(new Coord(0, 0))
           .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);
        galaxy.Starship!.EnergyLevel = 0;
        galaxy.Starship.ShieldLevel = 0;

        var result = game.RunRoutineMaintenance(1);
        var data = result.getResults<MaintResult>();

        Assert.Equal(eGameStatus.RanOutOfEnergy, data!.GameStatus);
    }

    [Fact]
    public void verify_GameStatus_NoEnergyShieldsAvailNotDamaged()
    {
        var rf = new RandomFactoryBuilder()
           .addEnemyLocation(new Coord(1,0))
           .addEnemyLocation(new Coord (6, 1))
           .addEnemyLocation(new Coord (4, 4))
           .setEnemyChance(100)
           .setStarbaseLocation(new Coord (1, 1))
           .setStarshipQuadrant(new Coord(0, 0))
           .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);
        galaxy.Starship!.EnergyLevel = 0;
        galaxy.Starship.ShieldLevel = 1000;

        var result = game.RunRoutineMaintenance(1);
        var data = result.getResults<MaintResult>();

        Assert.Equal(eGameStatus.OutOfEnergyShieldEnergyAvailable, data!.GameStatus);
    }

    [Fact]
    public void verify_GameStatus_NoEnergyShieldsAvailDamaged()
    {
        var rf = new RandomFactoryBuilder()
           .addEnemyLocation(new Coord (1, 0))
           .addEnemyLocation(new Coord (6, 1))
           .addEnemyLocation(new Coord (4, 4))
           .setEnemyChance(100)
           .setStarbaseLocation(new Coord (1, 1))
           .setStarshipQuadrant(new Coord(0, 0))
           .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);
        galaxy.Starship!.EnergyLevel = 0;
        galaxy.Starship.ShieldLevel = 1000;
        game.Galaxy.Starship.setDeviceDamage(eDevice.SHE, -3);

        var result = game.RunRoutineMaintenance(1);
        var data = result.getResults<MaintResult>();

        Assert.Equal(eGameStatus.RanOutOfEnergy, data!.GameStatus);
    }

    [Fact]
    public void verify_GameStatus_NoMoreEnemies()
    {
        var rf = new RandomFactoryBuilder()
           .setEnemyChance(0)
           .setStarbaseLocation(new Coord (1, 1))
           .setStarshipQuadrant(new Coord(0, 0))
           .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);
        game.Galaxy.MissionTimeDeadLine = game.Galaxy.MissionTime + 5;

        var result = game.RunRoutineMaintenance(1);
        var data = result.getResults<MaintResult>();

        Assert.Equal(eGameStatus.MissionOver, data!.GameStatus);
    }

    [Fact]
    public void verify_EnemiesFiredSwitchFalse_NoEnemiesFire()
    {
        var rf = new RandomFactoryBuilder()
           .addEnemyLocation(new Coord (1, 0))
           .addEnemyLocation(new Coord (6, 1))
           .addEnemyLocation(new Coord (4, 4))
           .setEnemyChance(100)
           .setStarBaseChance(100)
           .setEnemyMoveChance(0)
           .setStarbaseLocation(new Coord (1, 1))
           .setStarshipQuadrant(new Coord(0, 0))
           .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);
        game.Galaxy.debugSet(stopEnemyFire: true);
        var result = game.RunRoutineMaintenance(1);
        var data = result.getResults<MaintResult>();

        Assert.NotNull(data);
        Assert.NotNull(data.EnemiesFired);
        Assert.Empty(data!.EnemiesFired);
    }

    [Fact]
    public void verify_EnemiesMoveDuringMaint()
    {
        var origEnemyCoord = new Coord (1, 0);
        var newEnemyCoord = new Coord (4, 4);
        var rf = new RandomFactoryBuilder()
               .addEnemyLocation(origEnemyCoord)
               .addEnemyLocation(newEnemyCoord)
               .setEnemyChance(Global.EnemyZeroOne + 1)
               .setEnemyMoveChance(100)
               .setStarbaseLocation(new Coord (1, 1))
               .setStarshipQuadrant(new Coord(0, 0))
               .setStarshipSector(new Coord(0, 0));

        var game = GameBuilder.getGame(out var galaxy, rf);
        var result = game.RunRoutineMaintenance(1);
        var data = result.getResults<MaintResult>();

        Assert.NotNull(data);
        Assert.NotNull(data.EnemiesMoved);
        Assert.NotEmpty(data.EnemiesMoved);
        Assert.Single(data.EnemiesMoved);

        Assert.Equal(origEnemyCoord.X, data.EnemiesMoved.Single().EnemyCoord_Orig!.X);
        Assert.Equal(origEnemyCoord.Y, data.EnemiesMoved.Single().EnemyCoord_Orig!.Y);

        Assert.Equal(newEnemyCoord.X, data.EnemiesMoved.Single().EnemyCoord_Dest!.X);
        Assert.Equal(newEnemyCoord.Y, data.EnemiesMoved.Single().EnemyCoord_Dest!.Y);
    }
}
