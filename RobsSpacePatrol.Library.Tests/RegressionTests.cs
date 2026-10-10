using RobsSpacePatrol.Library.Tests;
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Factories;
using RobsSpacePatrol.Library.eNums;
using RobsSpacePatrol.Library.Utils;
using static RobsSpacePatrol.DTO.Library.Result;

namespace RobsSpacePatrol.CoreLogic.Tests;

public class RegressionTests
{
    [Fact]
    public void SHE_NegativeValue_DoesNotChangeEnergyOrShields()
    {
        var game = GameBuilder.getGame(out var galaxy);
        galaxy.Starship.EnergyLevel = 2400;
        galaxy.Starship.ShieldLevel = 600;

        var result = game.SHE(-1);
        var data = result.getResults<SHE>();

        Assert.Equal(eCommands.SHE, result.Command);
        Assert.Equal(eCommandResult.Error, result.eCommandResult);
        Assert.NotNull(data);
        Assert.Equal(Global.Input_ErrorMessage, data.ErrorMessage);
        Assert.Equal(2400, galaxy.Starship.EnergyLevel);
        Assert.Equal(600, galaxy.Starship.ShieldLevel);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void LAS_NonPositiveValue_DoesNotChangeEnergy(int energy)
    {
        var game = GameBuilder.getGame(out var galaxy);
        int startingEnergy = galaxy.Starship.EnergyLevel;

        var result = game.LAS(energy);

        Assert.Equal(eCommands.LAS, result.Command);
        Assert.Equal(eCommandResult.Error, result.eCommandResult);
        Assert.Null(result.Data);
        Assert.Equal(startingEnergy, galaxy.Starship.EnergyLevel);
    }

    [Fact]
    public void EnemyShieldHit_NonDivisibleEnergy_RoundsAfterDivision()
    {
        int result = CalculationUtilitiesV2.EnemyShieldHit(
            NumberOfEnemies: 2,
            EnergyOfShot: 7,
            DistanceToEnemy: 0,
            EnergyFactorToReducePerSector: Global.Sector2EnergyConversion);

        Assert.Equal(4, result);
    }

    [Fact]
    public void QS_ComparisonOperators_UseValueOrdering()
    {
        var sameReference = new QS(2, 3);
        var sameValue = new QS(2, 3);
        var lowerSector = new QS(1, 3);
        var higherSector = new QS(3, 3);
        var higherQuadrant = new QS(0, 4);

        Assert.False(sameReference > sameValue);
        Assert.False(sameReference < sameValue);
        Assert.True(sameReference >= sameValue);
        Assert.True(sameReference <= sameValue);
        Assert.True(lowerSector < sameReference);
        Assert.True(higherSector > sameReference);
        Assert.True(higherQuadrant > sameReference);

        QS? missing = null;
        QS? alsoMissing = null;
        Assert.False(missing > sameReference);
        Assert.False(missing < sameReference);
        Assert.False(missing >= alsoMissing);
        Assert.False(missing <= alsoMissing);
    }

    [Fact]
    public void RoutineMaintenance_CrossingDeadline_ReportsRanOutOfTimeImmediately()
    {
        var game = GameBuilder.getGame(out var galaxy, new RandomFactoryBuilder()
            .setEnemyChance(0)
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0)));
        galaxy.MissionTimeDeadLine = galaxy.MissionTime + 0.5;

        var result = game.RunRoutineMaintenance(1);
        var data = result.getResults<MaintResult>();

        Assert.NotNull(data);
        Assert.Equal(eGameStatus.RanOutOfTime, data.GameStatus);
        Assert.Equal(galaxy.MissionTime, data.newMissionTime);
    }

    [Fact]
    public void RoutineMaintenance_EnemyFireDestroysShip_ReportsFinalState()
    {
        var game = CreateGameWithEnemies(
            enemyCountChance: Global.EnemyZeroOne + 1,
            enemyHitAmount: 10,
            new Coord(1, 0));
        game.Galaxy.Starship.ShieldLevel = 5;

        var result = game.RunRoutineMaintenance(0);
        var data = result.getResults<MaintResult>();

        Assert.NotNull(data);
        Assert.True(game.Galaxy.Starship.isDestroyed);
        Assert.Equal(eGameStatus.StarshipDestroyed, data.GameStatus);
        Assert.Equal(game.Galaxy.Starship.ShieldLevel, data.StarshipShieldLevel);
        Assert.Equal(-5, data.StarshipShieldLevel);
    }

    [Fact]
    public void RoutineMaintenance_ZeroShieldsSurvivesUntilNextHit()
    {
        var game = CreateGameWithEnemies(
            enemyCountChance: Global.EnemyOneTwo + 1,
            enemyHitAmount: 10,
            new Coord(1, 0),
            new Coord(2, 0));
        game.Galaxy.Starship.ShieldLevel = 10;

        var result = game.RunRoutineMaintenance(0);
        var data = result.getResults<MaintResult>();

        Assert.NotNull(data?.EnemiesFired);
        Assert.Equal(2, data.EnemiesFired.Count);
        Assert.Equal(0, data.EnemiesFired[0].StarshipNewShieldAmount);
        Assert.False(data.EnemiesFired[0].StarshipDestroyed);
        Assert.Equal(-10, data.EnemiesFired[1].StarshipNewShieldAmount);
        Assert.True(data.EnemiesFired[1].StarshipDestroyed);
    }

    [Fact]
    public void RoutineMaintenance_AllDevicesAlreadyDamaged_DoesNotOverwriteDamage()
    {
        var randomFactory = new RandomFactoryBuilder()
            .addEnemyLocation(new Coord(1, 0))
            .setEnemyChance(Global.EnemyZeroOne + 1)
            .setEnemyFiredAmount(10)
            .setDamagedDeviceChance(100)
            .setDamagedDevice(0)
            .setDamagedDeviceAmount(2)
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));
        var game = GameBuilder.getGame(out var galaxy, randomFactory);
        game.Galaxy.debugSet(skipEnemyMove: true);
        galaxy.Starship.ShieldLevel = 100;
        galaxy.Starship.Devices.ForEach(device => device.DamageLevel = -5);

        var result = game.RunRoutineMaintenance(0);
        var data = result.getResults<MaintResult>();

        Assert.All(galaxy.Starship.Devices, device => Assert.Equal(-5, device.DamageLevel));
        Assert.NotNull(data?.EnemiesFired);
        Assert.Single(data.EnemiesFired);
        Assert.Null(data.EnemiesFired[0].DeviceDamaged);
        Assert.Equal(0, data.EnemiesFired[0].DeviceDamangedAmount);
    }

    [Fact]
    public void CurrentQuadrantFactory_RepeatedOccupiedCoordinates_FallsBackToEmptySector()
    {
        var occupied = new Sector(new Coord(0, 0), Global.SectorStar);
        var empty = new Sector(new Coord(1, 0), Global.SectorEmpty);
        var randomFactory = new RandomFactoryBuilder()
            .addEnemyLocation(occupied.Coord)
            .GetFactories();

        var result = new CurrentQuadrantFactory().getRandomEmptySector(
            randomFactory,
            [occupied, empty],
            eRandomCoordType.EnemyLocation);

        Assert.Same(empty, result);
    }

    [Fact]
    public void RandomFactory_DamagedDevice_UsesLastValidIndexAtUpperBound()
    {
        var factory = new RandomFactory(new UpperBoundRandom());

        int result = factory.GetRandomInteger(eRandomIntType.DamagedDevice);

        Assert.Equal(Enum.GetValues<eDevice>().Length - 1, result);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    [InlineData(0)]
    [InlineData(9)]
    public void TOR_InvalidDirection_DoesNotConsumeTorpedo(double direction)
    {
        var game = GameBuilder.getGame(out var galaxy);
        int startingTorpedoes = galaxy.Starship.TorpedosRemaining;

        var result = game.TOR(direction);

        Assert.Equal(eCommands.TOR, result.Command);
        Assert.Equal(eCommandResult.Error, result.eCommandResult);
        Assert.Null(result.Data);
        Assert.Equal(startingTorpedoes, galaxy.Starship.TorpedosRemaining);
    }

    [Fact]
    public void LAS_DestroyingEnemy_KeepsQuadrantCountConsistent()
    {
        var randomFactory = new RandomFactoryBuilder()
            .addEnemyLocation(new Coord(1, 0))
            .setEnemyChance(Global.EnemyZeroOne + 1)
            .setEnemyShieldLevel(0)
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));
        var game = GameBuilder.getGame(out var galaxy, randomFactory);

        var result = game.LAS(100);
        var quadrant = galaxy.GetQuadrantByCoord(galaxy.CurrentQuadrant.Coord);
        int enemiesInSectors = galaxy.CurrentQuadrant.Sectors.Count(sector => sector.HasEnemy);

        Assert.Equal(eCommandResult.OK, result.eCommandResult);
        Assert.NotNull(quadrant);
        Assert.Equal(enemiesInSectors, quadrant.NumberOfEnemies);
        Assert.Equal(0, quadrant.NumberOfEnemies);
    }

    [Fact]
    public void TOR_DestroyingEnemy_KeepsQuadrantCountConsistent()
    {
        var randomFactory = new RandomFactoryBuilder()
            .addEnemyLocation(new Coord(1, 0))
            .setEnemyChance(Global.EnemyZeroOne + 1)
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));
        var game = GameBuilder.getGame(out var galaxy, randomFactory);

        var result = game.TOR(1);
        var quadrant = galaxy.GetQuadrantByCoord(galaxy.CurrentQuadrant.Coord);
        int enemiesInSectors = galaxy.CurrentQuadrant.Sectors.Count(sector => sector.HasEnemy);

        Assert.Equal(eCommandResult.OK, result.eCommandResult);
        Assert.NotNull(quadrant);
        Assert.Equal(enemiesInSectors, quadrant.NumberOfEnemies);
        Assert.Equal(0, quadrant.NumberOfEnemies);
    }

    private static RobsSpacePatrol.Library.Game.Game CreateGameWithEnemies(
        int enemyCountChance,
        int enemyHitAmount,
        params Coord[] enemyLocations)
    {
        var builder = new RandomFactoryBuilder()
            .setEnemyChance(enemyCountChance)
            .setEnemyFiredAmount(enemyHitAmount)
            .setEnemyMoveChance(0)
            .setDamagedDeviceChance(0)
            .setStarshipQuadrant(new Coord(0, 0))
            .setStarshipSector(new Coord(0, 0));

        foreach (var location in enemyLocations)
            builder.addEnemyLocation(location);

        var game = GameBuilder.getGame(builder);
        game.Galaxy.debugSet(skipEnemyMove: true);
        return game;
    }

    private sealed class UpperBoundRandom : Random
    {
        public override int Next(int minValue, int maxValue) => maxValue - 1;
    }
}
