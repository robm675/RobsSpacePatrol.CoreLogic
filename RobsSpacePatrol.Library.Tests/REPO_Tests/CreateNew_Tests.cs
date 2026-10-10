
namespace RobsSpacePatrol.CoreLogic.Tests.REPO_Tests;

public class CreateNew_Tests
{
    const int StarshipQX = 2;
    const int StarshipQY = 3;

    [Fact]
    public void verify_GalaxyCreated()
    {
        var factory = new RandomFactoryBuilder()
             .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
             .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.NotNull(galaxy);
    }
    [Fact]
    public void verify_QuadrantsNotNull()
    {
        var factory = new RandomFactoryBuilder()
             .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
             .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.NotNull(galaxy.Quadrants);
    }
    [Fact]
    public void verify_Quadrants_Count()
    {
        var factory = new RandomFactoryBuilder()
             .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
             .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.Equal(GlobalValues.MaxQuadrantsXY * GlobalValues.MaxQuadrantsXY, galaxy.Quadrants?.Count);
    }
    [Fact]
    public void verify_Quadrants_StarBaseCount()
    {
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .setStarbaseLocation(new CoordDTO { X = 7, Y = 7 })
            .setStarBaseChance(100)
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.Equal(GlobalValues.MaxQuadrantsXY * GlobalValues.MaxQuadrantsXY, galaxy?.Quadrants?.Count(z => z.HasStarBase));
    }
    [Fact]
    public void verify_Quadrants_StarCount()
    {
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .setStarQuantity(1)
            .addStarLocation(new CoordDTO { X = 1, Y = 2 })
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.Equal(GlobalValues.MaxQuadrantsXY * GlobalValues.MaxQuadrantsXY, galaxy?.Quadrants?.Sum(z => z.NumberOfStars));
    }
    [Fact]
    public void verify_Quadrants_EnemyCount_1()
    {
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .setEnemyChance(GlobalValues.EnemyZeroOne + 1)
            .addEnemyLocation(new CoordDTO { X = 1, Y = 2 })
            .addEnemyLocation(new CoordDTO { X = 1, Y = 3 })
            .addEnemyLocation(new CoordDTO { X = 1, Y = 4 })
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.Equal(GlobalValues.MaxQuadrantsXY * GlobalValues.MaxQuadrantsXY, galaxy?.Quadrants?.Sum(z => z.NumberOfEnemies));
    }
    [Fact]
    public void verify_Quadrants_EnemyCount_2()
    {
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .setEnemyChance(GlobalValues.EnemyOneTwo + 1)
            .addEnemyLocation(new CoordDTO { X = 1, Y = 2 })
            .addEnemyLocation(new CoordDTO { X = 1, Y = 3 })
            .addEnemyLocation(new CoordDTO { X = 1, Y = 4 })
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.Equal(2 * GlobalValues.MaxQuadrantsXY * GlobalValues.MaxQuadrantsXY, galaxy?.Quadrants?.Sum(z => z.NumberOfEnemies));
    }
    [Fact]
    public void verify_Quadrants_EnemyCount_3()
    {
        var factory = new RandomFactoryBuilder()
            .addEnemyLocation(new CoordDTO { X = 1, Y = 2 })
            .addEnemyLocation(new CoordDTO { X = 1, Y = 3 })
            .addEnemyLocation(new CoordDTO { X = 1, Y = 4 })
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .setEnemyChance(GlobalValues.EnemyTwoThree + 1)
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.Equal(3 * GlobalValues.MaxQuadrantsXY * GlobalValues.MaxQuadrantsXY, galaxy?.Quadrants?.Sum(z => z.NumberOfEnemies));
    }
    [Fact]
    public void verify_CurrentQuadrantNotNull()
    {
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.NotNull(galaxy.CurrentQuadrant);
    }
    [Fact]
    public void verify_StarshipQuadrant()
    {
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.Equal(StarshipQX, galaxy?.CurrentQuadrant?.Coord?.X);
        Assert.Equal(StarshipQY, galaxy?.CurrentQuadrant?.Coord?.Y);
    }
    [Fact]
    public void verify_StarshipNotNull()
    {
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.NotNull(galaxy.Starship);
    }
    [Fact]
    public void verify_StarshipNewlyCreatedValues()
    {
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.Equal(GlobalValues.maxStarshipEnergy, galaxy?.Starship?.EnergyLevel);
        Assert.Equal(false, galaxy?.Starship?.isDocked);
        Assert.Equal(false, galaxy?.Starship?.isDestroyed);
        Assert.Equal(0, galaxy?.Starship?.ShieldLevel);
        Assert.Equal(GlobalValues.maxStarshipTorpedos, galaxy?.Starship?.TorpedosRemaining);
        Assert.NotNull(galaxy?.Starship?.Devices);
    }
    [Fact]
    public void verify_StarshipSectorCoord()
    {
        int x = 6;
        int y = 7;
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .setStarshipSector(new CoordDTO { X = x, Y = y })
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.Equal(x, galaxy?.Starship?.StarshipSectorCoord?.X);
        Assert.Equal(y, galaxy?.Starship?.StarshipSectorCoord?.Y);
    }
    [Fact]
    public void verify_MissionTime()
    {
        int MissionTime = 2439;
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .setStartingMissionTime(MissionTime)
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.Equal(MissionTime, galaxy.MissionTime);
    }
    [Fact]
    public void verify_MissionTimeDeadline()
    {
        int MissionTime = 2539;
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .addEnemyLocation(new CoordDTO { X = 1, Y = 2 })
            .setEnemyChance(GlobalValues.EnemyZeroOne + 1)
            .setStartingMissionTime(MissionTime)
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.Equal(MissionTime + 64 * GlobalValues.MissionTimeUnitsPerEnemyRatio, galaxy.MissionTimeDeadLine);
    }
    [Fact]
    public void verify_CurrentQuadrant_SectorsNotNull()
    {
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.NotNull(galaxy?.CurrentQuadrant?.Sectors);
    }
    [Fact]
    public void verify_CurrentQuadrant_SectorsCount()
    {
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.Equal(GlobalValues.MaxSectorsXY * GlobalValues.MaxSectorsXY, galaxy?.CurrentQuadrant?.Sectors?.Count);
    }
    [Fact]
    public void verify_CurrentQuadrant_EmptySectorCount()
    {
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        //-1 because the starship takes one of the 64
        Assert.Equal(
            GlobalValues.MaxSectorsXY * GlobalValues.MaxSectorsXY - 1,
            galaxy?.CurrentQuadrant?.Sectors?.Count(
                z => z.SectorContents?.SectorContents == GlobalValues.SectorEmpty));
    }
    [Fact]
    public void verify_CurrentQuadrant_StarCount()
    {
        int starNum = 4;
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .setStarQuantity(starNum)
            .addStarLocation(new CoordDTO { X = 1, Y = 2 })
            .addStarLocation(new CoordDTO { X = 1, Y = 3 })
            .addStarLocation(new CoordDTO { X = 1, Y = 4 })
            .addStarLocation(new CoordDTO { X = 1, Y = 5 })
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.Equal(
            starNum,
            galaxy?.CurrentQuadrant?.Sectors?.Count(
                z => z.SectorContents?.SectorContents == GlobalValues.SectorStar));
    }
    [Fact]
    public void verify_CurrentQuadrant_StarBase()
    {
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .setStarBaseChance(100)
            .setStarbaseLocation(new CoordDTO { X = 1, Y = 2 })
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.Equal(
            1,
            galaxy?.CurrentQuadrant?.Sectors?.Count(
                z => z.SectorContents?.SectorContents == GlobalValues.SectorStarBase));
    }
    [Fact]
    public void verify_CurrentQuadrant_Enemy_0()
    {
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .setEnemyChance(GlobalValues.EnemyZeroOne)
            .addEnemyLocation(new CoordDTO { X = 1, Y = 2 })
            .addEnemyLocation(new CoordDTO { X = 1, Y = 3 })
            .addEnemyLocation(new CoordDTO { X = 1, Y = 4 })
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.Equal(
            0,
            galaxy?.CurrentQuadrant?.Sectors?.Count(
                z => z.SectorContents?.SectorContents == GlobalValues.SectorEnemy));
    }
    [Fact]
    public void verify_CurrentQuadrant_Enemy_1()
    {
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .setEnemyChance(GlobalValues.EnemyOneTwo)
            .addEnemyLocation(new CoordDTO { X = 1, Y = 2 })
            .addEnemyLocation(new CoordDTO { X = 1, Y = 3 })
            .addEnemyLocation(new CoordDTO { X = 1, Y = 4 })
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.Equal(
            1,
            galaxy?.CurrentQuadrant?.Sectors?.Count(
                z => z.SectorContents?.SectorContents == GlobalValues.SectorEnemy));
    }
    [Fact]
    public void verify_CurrentQuadrant_Enemy_2()
    {
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .setEnemyChance(GlobalValues.EnemyOneTwo + 1)
            .addEnemyLocation(new CoordDTO { X = 1, Y = 2 })
            .addEnemyLocation(new CoordDTO { X = 1, Y = 3 })
            .addEnemyLocation(new CoordDTO { X = 1, Y = 4 })
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.Equal(
            2,
            galaxy?.CurrentQuadrant?.Sectors?.Count(
                z => z.SectorContents?.SectorContents == GlobalValues.SectorEnemy));
    }
    [Fact]
    public void verify_CurrentQuadrant_Enemy_3()
    {
        var factory = new RandomFactoryBuilder()
            .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
            .setEnemyChance(GlobalValues.EnemyTwoThree + 1)
            .addEnemyLocation(new CoordDTO { X = 1, Y = 2 })
            .addEnemyLocation(new CoordDTO { X = 1, Y = 3 })
            .addEnemyLocation(new CoordDTO { X = 1, Y = 4 })
            .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());

        Assert.Equal(
            3,
            galaxy?.CurrentQuadrant?.Sectors?.Count(
                z => z.SectorContents?.SectorContents == GlobalValues.SectorEnemy));
    }
}
