
namespace RobsSpacePatrol.CoreLogic.Tests.REPO_Tests;

public class CurrentQuadrantFactor_Tests
{
    CompareLogic getComp()
    {
        return RobsSpacePatrol.Global.GlobalValues.getComparer();
    }

    [Fact]
    public void Verify_NullQuadrantCoord()
    {
        var entSector = new CoordDTO { X = 0, Y = 0 };
        var rf = new RandomFactoryBuilder()
         .setStarshipQuadrant(new CoordDTO { X = 0, Y = 0 })
         .setStarshipSector(entSector)
         .GetFactories();

        var qf = new CurrentQuadrantFactory();
        var q = new QuadrantDTO();
        var cq = qf.createCurrentQuadrant(q, rf, entSector);

        Assert.True(getComp().Compare(cq, new CurrentQuadrantDTO()).AreEqual);
    }

    [Fact]
    public void Verify_CQCoord()
    {
        var entSector = new CoordDTO { X = 3, Y = 4 };
        var entQuad = new CoordDTO { X = 1, Y = 2 };
        var rf = new RandomFactoryBuilder()
         .setStarshipQuadrant(entQuad)
         .setStarshipSector(entSector)
         .GetFactories();

        var qf = new CurrentQuadrantFactory();
        var q = new QuadrantDTO() { Coord = entQuad };
        var cq = qf.createCurrentQuadrant(q, rf, entSector);

        Assert.True(getComp().Compare(q.Coord, cq.Coord).AreEqual);
    }

    [Fact]
    public void Verify_SectorsNotNull()
    {
        var entSector = new CoordDTO { X = 3, Y = 4 };
        var entQuad = new CoordDTO { X = 1, Y = 2 };
        var rf = new RandomFactoryBuilder()
         .setStarshipQuadrant(entQuad)
         .setStarshipSector(entSector)
         .GetFactories();

        var qf = new CurrentQuadrantFactory();
        var q = new QuadrantDTO() { Coord = entQuad };
        var cq = qf.createCurrentQuadrant(q, rf, entSector);

        Assert.NotNull(cq.Sectors);
    }

    [Fact]
    public void Verify_SectorsCorrectQuantity()
    {
        var entSector = new CoordDTO { X = 3, Y = 4 };
        var entQuad = new CoordDTO { X = 1, Y = 2 };
        var rf = new RandomFactoryBuilder()
         .setStarshipQuadrant(entQuad)
         .setStarshipSector(entSector)
         .GetFactories();

        var qf = new CurrentQuadrantFactory();
        var q = new QuadrantDTO() { Coord = entQuad };
        var cq = qf.createCurrentQuadrant(q, rf, entSector);

        Assert.Equal(GlobalValues.MaxSectorsXY * GlobalValues.MaxSectorsXY, cq.Sectors!.Count);
    }

    [Fact]
    public void Verify_SectorsAreEmpty()
    {
        var entSector = new CoordDTO { X = 3, Y = 4 };
        var entQuad = new CoordDTO { X = 1, Y = 2 };
        var rf = new RandomFactoryBuilder()
         .setStarshipQuadrant(entQuad)
         .setStarshipSector(entSector)
         .GetFactories();

        var qf = new CurrentQuadrantFactory();
        var q = new QuadrantDTO() { Coord = entQuad };
        var cq = qf.createCurrentQuadrant(q, rf, entSector);

        int totalSectors = cq.Sectors!.Count;
        int emptyCount = cq.Sectors.Count(z => z.SectorContents.SectorContents == GlobalValues.SectorEmpty);
        Assert.Equal(totalSectors - 1, emptyCount);
    }

    [Fact]
    public void Verify_StarshipCorrectSector()
    {
        var entSector = new CoordDTO { X = 3, Y = 4 };
        var entQuad = new CoordDTO { X = 1, Y = 2 };
        var rf = new RandomFactoryBuilder()
         .setStarshipQuadrant(entQuad)
         .setStarshipSector(entSector)
         .GetFactories();

        var qf = new CurrentQuadrantFactory();
        var q = new QuadrantDTO() { Coord = entQuad };
        var cq = qf.createCurrentQuadrant(q, rf, entSector);

        var entCQSect = cq.Sectors!.First(z => z.SectorContents.SectorContents == GlobalValues.SectorStarship);
        Assert.True(getComp().Compare(entCQSect.Coord, entSector).AreEqual);
    }

    [Fact]
    public void Verify_StarQuantity()
    {
        var entSector = new CoordDTO { X = 2, Y = 4 };
        var entQuad = new CoordDTO { X = 1, Y = 2 };
        int numStars = 9;
        var rf = new RandomFactoryBuilder()
            .setStarQuantity(numStars)
            .addStarLocation(new CoordDTO { X = 3, Y = 0 })
            .addStarLocation(new CoordDTO { X = 3, Y = 1 })
            .addStarLocation(new CoordDTO { X = 3, Y = 2 })
            .addStarLocation(new CoordDTO { X = 3, Y = 3 })
            .addStarLocation(new CoordDTO { X = 3, Y = 4 })
            .addStarLocation(new CoordDTO { X = 3, Y = 5 })
            .addStarLocation(new CoordDTO { X = 3, Y = 6 })
            .addStarLocation(new CoordDTO { X = 3, Y = 7 })
            .addStarLocation(new CoordDTO { X = 4, Y = 0 })
            .addStarLocation(new CoordDTO { X = 4, Y = 1 })
            .setStarshipQuadrant(entQuad)
            .setStarshipSector(entSector)
            .GetFactories();

        var qf = new CurrentQuadrantFactory();
        var q = new QuadrantDTO() { Coord = entQuad, NumberOfStars = numStars };
        var cq = qf.createCurrentQuadrant(q, rf, entSector);

        Assert.Equal(numStars, cq.Sectors!.Count(z => z.SectorContents.SectorContents == GlobalValues.SectorStar));
    }

    [Fact]
    public void Verify_StarBase()
    {
        var entSector = new CoordDTO { X = 2, Y = 4 };
        var entQuad = new CoordDTO { X = 1, Y = 2 };
        var rf = new RandomFactoryBuilder()
            .setStarBaseChance(100)
            .setStarbaseLocation(new CoordDTO { X = 4, Y = 1 })
            .setStarshipQuadrant(entQuad)
            .setStarshipSector(entSector)
            .GetFactories();

        var qf = new CurrentQuadrantFactory();
        var q = new QuadrantDTO() { Coord = entQuad, HasStarBase = true };
        var cq = qf.createCurrentQuadrant(q, rf, entSector);

        Assert.Equal(1, cq.Sectors!.Count(z => z.SectorContents.SectorContents == GlobalValues.SectorStarBase));
    }

    [Fact]
    public void Verify_HasBeenExplored()
    {
        var entSector = new CoordDTO { X = 2, Y = 4 };
        var entQuad = new CoordDTO { X = 1, Y = 2 };
        var rf = new RandomFactoryBuilder()
            .setStarBaseChance(100)
            .setStarbaseLocation(new CoordDTO { X = 4, Y = 1 })
            .setStarshipQuadrant(entQuad)
            .setStarshipSector(entSector)
            .GetFactories();

        var qf = new CurrentQuadrantFactory();
        var q = new QuadrantDTO() { Coord = entQuad, HasStarBase = true };
        var cq = qf.createCurrentQuadrant(q, rf, entSector);

        Assert.True(q.HasBeenExplored);
    }

    [Fact]
    public void Verify_EnemyQuantity()
    {
        var entSector = new CoordDTO { X = 2, Y = 4 };
        var entQuad = new CoordDTO { X = 1, Y = 2 };
        int numEnemies = 3;
        var rf = new RandomFactoryBuilder()
            .setEnemyChance(100)
            .addEnemyLocation(new CoordDTO { X = 3, Y = 0 })
            .addEnemyLocation(new CoordDTO { X = 3, Y = 1 })
            .addEnemyLocation(new CoordDTO { X = 3, Y = 2 })
            .addEnemyLocation(new CoordDTO { X = 3, Y = 3 })
            .setStarshipQuadrant(entQuad)
            .setStarshipSector(entSector)
            .GetFactories();

        var qf = new CurrentQuadrantFactory();
        var q = new QuadrantDTO() { Coord = entQuad, NumberOfEnemies = numEnemies };
        var cq = qf.createCurrentQuadrant(q, rf, entSector);

        Assert.Equal(numEnemies, cq.Sectors!.Count(z => z.SectorContents.SectorContents == GlobalValues.SectorEnemy));
    }
}
