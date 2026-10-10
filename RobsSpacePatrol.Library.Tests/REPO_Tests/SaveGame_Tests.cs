
namespace RobsSpacePatrol.CoreLogic.Tests.REPO_Tests;

public class SaveGame_Tests
{
    const int StarshipQX = 2;
    const int StarshipQY = 3;


    private CompareLogic getComparer()
    {
        var compareConfig = new ComparisonConfig();
        var _cl = new CompareLogic(compareConfig);
        return _cl;
    }

    public SaveGame_Tests()
    {
        var db = new RobsSpacePatrolDBContext();
        var REPO = new RobsSpacePatrolREPO(db);
        var saveGames = REPO.getGalaxies();

        foreach (var sg in saveGames)
            REPO.deleteGalaxy(sg.ID);

        if (REPO.getTotalRecordCount() != 0)
            throw new Exception("All records were NOT deleted");
    }

    [Fact(Skip = "TBD")]
    public void verify_SaveGame_Same()
    {
        var factory = new RandomFactoryBuilder()
             .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
             .setEnemyChance(GlobalValues.EnemyTwoThree + 1)
             .addEnemyLocation(new CoordDTO { X = 1, Y = 2 })
             .addEnemyLocation(new CoordDTO { X = 1, Y = 3 })
             .addEnemyLocation(new CoordDTO { X = 1, Y = 4 })
             .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());
        var db = new RobsSpacePatrolDBContext();
        var REPO = new RobsSpacePatrolREPO(db);
        CompareLogic cl = getComparer();


        REPO.SaveGalaxy(galaxy);
        var galaxyFromDB = REPO.LoadGalaxy(galaxy.ID);

        var results = cl.Compare(galaxy, galaxyFromDB);

        if (!results.AreEqual)
            Console.WriteLine(results.DifferencesString);

        Assert.True(results.AreEqual);
    }

    [Fact(Skip = "TBD")]
    public void verify_SaveGame_Different()
    {
        var factory = new RandomFactoryBuilder()
             .setStarshipQuadrant(new CoordDTO { ID = Guid.NewGuid(), X = StarshipQX, Y = StarshipQY })
             .setEnemyChance(GlobalValues.EnemyTwoThree + 1)
             .addEnemyLocation(new CoordDTO { X = 1, Y = 2 })
             .addEnemyLocation(new CoordDTO { X = 1, Y = 3 })
             .addEnemyLocation(new CoordDTO { X = 1, Y = 4 })
             .GetFactories();
        var galaxy = new OtherFactories().createGalaxy(factory, new CurrentQuadrantFactory());
        var db = new RobsSpacePatrolDBContext();
        var REPO = new RobsSpacePatrolREPO(db);
        CompareLogic cl = getComparer();


        REPO.SaveGalaxy(galaxy);
        var galaxyFromDB = REPO.LoadGalaxy(galaxy.ID);

        galaxyFromDB.Description = "SOMETHING DIFFERENT";

        var results = cl.Compare(galaxy, galaxyFromDB);

        if (!results.AreEqual)
            Console.WriteLine(results.DifferencesString);

        Assert.False(results.AreEqual);

    }
}
