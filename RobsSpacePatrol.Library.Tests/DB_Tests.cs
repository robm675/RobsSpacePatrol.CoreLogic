using RobsSpacePatrol.Library.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RobsSpacePatrol.Library.Utils;
using System.Text.Json;

namespace RobsSpacePatrol.CoreLogic.Tests;


public class DB_Tests
{
    [Fact]
    public void TestSerialization()
    {
        var game = GameBuilder.getGame();
        var json = DB.GetJsonFromGalaxy((Galaxy)game.Galaxy);

        Assert.NotNull(json);
        Assert.NotEmpty(json);
    }

    [Fact]
    public void TestDeserialization()
    {
        var game = GameBuilder.getGame();
        Galaxy tempGalaxy = (Galaxy)game.Galaxy;
        var json = DB.GetJsonFromGalaxy(tempGalaxy);

        var galaxy = DB.GetGalaxyFromJson(json);

        var galaxyDiffs = IsGalaxySame(game.Galaxy, galaxy!);


        foreach (var gd in galaxyDiffs)
        {
            Console.WriteLine(gd.objectType);
            Console.WriteLine(gd.value1);
            Console.WriteLine(gd.value2);
            Console.WriteLine();
        }


        Assert.Empty(galaxyDiffs);
    }

    [Fact]
    public void TestComparer()
    {
        var game = GameBuilder.getGame();
        var json = DB.GetJsonFromGalaxy((Galaxy)game.Galaxy);

        var galaxy = DB.GetGalaxyFromJson(json);
        Assert.NotNull(galaxy);
        galaxy.MissionTime = 0;
        galaxy.CurrentQuadrant.Coord = new Coord(-1, -1);
        galaxy.CurrentQuadrant.Sectors.First(z => z.Coord == new Coord(3, 4)).SectorContents = "DORK";
        galaxy.Quadrants.First(z => z.Coord == new Coord(4, 4)).NumberOfStars = 1234;
        galaxy.debugSet(true, true, true);


        var galaxyDiffs = IsGalaxySame(game.Galaxy, galaxy!);

        var target1 = galaxyDiffs.FirstOrDefault(z => z.objectType == nameof(Galaxy.MissionTime));
        Assert.NotNull(target1);
        Assert.Equal("0", target1.value2);
        Assert.Equal(galaxy.MissionTime.ToString(), target1.value2);

        var target2 = galaxyDiffs.FirstOrDefault(z => z.objectType == nameof(Coord.X));
        Assert.NotNull(target2);
        Assert.Equal("-1", target2.value2);
        Assert.Equal(galaxy.CurrentQuadrant.Coord.X.ToString(), target2.value2);

        var target3 = galaxyDiffs.FirstOrDefault(z => z.objectType == nameof(Coord.Y));
        Assert.NotNull(target3);
        Assert.Equal("-1", target3.value2);
        Assert.Equal(galaxy.CurrentQuadrant.Coord.Y.ToString(), target3.value2);

        var target4 = galaxyDiffs.FirstOrDefault(z => z.objectType == nameof(Sector.SectorContents));
        var sector = game.Galaxy.CurrentQuadrant.GetSectorByCoord(new Coord(3, 4));
        Assert.NotNull(target4);
        Assert.Equal("DORK", target4.value2);
        Assert.Equal(sector.SectorContents, target4.value1);

        var target5 = galaxyDiffs.FirstOrDefault(z => z.objectType == nameof(Quadrant.NumberOfStars));
        var quad = game.Galaxy.GetQuadrantByCoord(new Coord(4, 4));
        Assert.NotNull(target5);
        Assert.NotNull(quad);
        Assert.Equal("1234", target5.value2);
        Assert.Equal(quad.NumberOfStars.ToString(), target5.value1);

    }

    [Fact]
    public void GetGalaxyFromJson_InvalidJson_ThrowsInvalidDataException()
    {
        Assert.Throws<InvalidDataException>(
            () => DB.GetGalaxyFromJson("{ invalid json"));
    }

    [Fact]
    public void GetGalaxyFromJson_NullJsonValue_ThrowsInvalidDataException()
    {
        Assert.Throws<InvalidDataException>(
            () => DB.GetGalaxyFromJson("null"));
    }

    [Fact]
    public void GetGalaxyFromJson_EmptyInput_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => DB.GetGalaxyFromJson(""));
    }

    public record Diff(string objectType, string? value1, string? value2);
    private List<Diff> IsGalaxySame(IGalaxy left, IGalaxy right)
    {
        List<Diff> listOut = new();

        if (left.stopEnemyFireSW != right.stopEnemyFireSW) listOut.Add(new(nameof(left.stopEnemyFireSW), left.stopEnemyFireSW.ToString(), right.stopEnemyFireSW.ToString()));
        if (left.skipEnemyMoveSW != right.skipEnemyMoveSW) listOut.Add(new(nameof(left.skipEnemyMoveSW), left.skipEnemyMoveSW.ToString(), right.skipEnemyMoveSW.ToString()));
        if (left.forceEnemyMoveSW != right.forceEnemyMoveSW) listOut.Add(new(nameof(left.forceEnemyMoveSW), left.forceEnemyMoveSW.ToString(), right.forceEnemyMoveSW.ToString()));

        if (left.MissionTime != right.MissionTime) listOut.Add(new(nameof(left.MissionTime), left.MissionTime.ToString(), right.MissionTime.ToString()));

        if (left.MissionTimeDeadLine != right.MissionTimeDeadLine) listOut.Add(new(nameof(left.MissionTimeDeadLine), left.MissionTimeDeadLine.ToString(), right.MissionTimeDeadLine.ToString()));

        if (left.Starship.TorpedosRemaining != right.Starship.TorpedosRemaining)
            listOut.Add(new(nameof(Starship.TorpedosRemaining), left.Starship.TorpedosRemaining.ToString(), right.Starship.TorpedosRemaining.ToString()));

        if (left.Starship.isDocked != right.Starship.isDocked)
            listOut.Add(new(nameof(Starship.isDocked), left.Starship.isDocked.ToString(), right.Starship.isDocked.ToString()));

        if (left.Starship.ShieldLevel != right.Starship.ShieldLevel)
            listOut.Add(new(nameof(Starship.ShieldLevel), left.Starship.ShieldLevel.ToString(), right.Starship.ShieldLevel.ToString()));

        if (left.Starship.EnergyLevel != right.Starship.EnergyLevel)
            listOut.Add(new(nameof(Starship.EnergyLevel), left.Starship.EnergyLevel.ToString(), right.Starship.EnergyLevel.ToString()));

        if (left.Starship.isDestroyed != right.Starship.isDestroyed)
            listOut.Add(new(nameof(Starship.isDestroyed), left.Starship.isDestroyed.ToString(), right.Starship.isDestroyed.ToString()));

        foreach (var dev in left.Starship.Devices)
        {
            var dev2 = right.Starship.Devices.FirstOrDefault(z => z.Name == dev.Name);
            if (dev.DamageLevel != dev2!.DamageLevel)
                listOut.Add(new(nameof(Device.DamageLevel), dev.DamageLevel.ToString(), dev2.DamageLevel.ToString()));
        }

        if (left.CurrentQuadrant.Coord.X != right.CurrentQuadrant.Coord.X)
            listOut.Add(new(nameof(CurrentQuadrant.Coord.X), left.CurrentQuadrant.Coord.X.ToString(), right.CurrentQuadrant.Coord.X.ToString()));

        if (left.CurrentQuadrant.Coord.Y != right.CurrentQuadrant.Coord.Y)
            listOut.Add(new(nameof(CurrentQuadrant.Coord.Y), left.CurrentQuadrant.Coord.Y.ToString(), right.CurrentQuadrant.Coord.Y.ToString()));


        foreach (var dev in left.CurrentQuadrant.Sectors)
        {
            var dev2 = right.CurrentQuadrant.Sectors.FirstOrDefault(z => z.Coord == dev.Coord);

            if (dev.Enemy == null && dev2?.Enemy != null)
                listOut.Add(new(nameof(Enemy), "Enemy 2 Null", "Enemy 2 Not Null"));
            if (dev.Enemy != null && dev2?.Enemy == null)
                listOut.Add(new(nameof(Enemy), "Enemy not 2 Null", "Enemy 2 Null"));

            if (dev.Enemy != null && dev2?.Enemy != null)
            {
                if (dev.Enemy?.ShieldLevel != dev2?.Enemy?.ShieldLevel)
                    listOut.Add(new(nameof(Enemy.ShieldLevel), dev.Enemy?.ShieldLevel.ToString(), dev2?.Enemy?.ShieldLevel.ToString()));
            }

            if (dev.SectorContents != dev2?.SectorContents)
                listOut.Add(new(nameof(Sector.SectorContents), dev.SectorContents.ToString(), dev2?.SectorContents.ToString()));
        }

        foreach (var q1 in left.Quadrants)
        {
            var q2 = right.Quadrants.FirstOrDefault(z => z.Coord == q1.Coord);
            if (q2 == null)
                listOut.Add(new($"Quadrant with coord {q1.Coord} not found in right", "", ""));

            if (q2 != null)
            {
                if (q1.HasStarBase != q2.HasStarBase)
                    listOut.Add(new(nameof(Quadrant.HasStarBase), q1.HasStarBase.ToString(), q2.HasStarBase.ToString()));

                if (q1.HasBeenExplored != q2.HasBeenExplored)
                    listOut.Add(new(nameof(Quadrant.HasBeenExplored), q1.HasBeenExplored.ToString(), q2.HasBeenExplored.ToString()));

                if (q1.NumberOfEnemies != q2.NumberOfEnemies)
                    listOut.Add(new(nameof(Quadrant.NumberOfEnemies), q1.NumberOfEnemies.ToString(), q2.NumberOfEnemies.ToString()));

                if (q1.NumberOfStars != q2.NumberOfStars)
                    listOut.Add(new(nameof(Quadrant.NumberOfStars), q1.NumberOfStars.ToString(), q2.NumberOfStars.ToString()));
            }
        }




        return listOut;
    }
}
