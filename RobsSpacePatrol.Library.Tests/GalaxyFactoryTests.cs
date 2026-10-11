using RobsSpacePatrol.Library.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobsSpacePatrol.CoreLogic.Tests;

public class GalaxyFactoryTests
{
    [Fact]
    public void testStuff()
    {
        var rf = new RandomFactoryBuilder()
                .addEnemyLocation(new Coord(6, 1))
                .addEnemyLocation(new Coord(4, 4))
                .addEnemyLocation(new Coord(1, 0))
                .setStarshipQuadrant(new Coord(1, 2))
                .setStarshipSector(new Coord(1, 1))
                .setStarBaseChance(0)
                .setStarQuantity(0)
                .setEnemyChance(100);



        var game = GameBuilder.getGame(rf);


        var total = game.Galaxy.Quadrants!.Sum(z => z.NumberOfEnemies);
        Assert.Equal(192, total);
    }

    [Fact]
    public void CreateGalaxy_NoRandomStarbases_AddsOneStarbase()
    {
        var random = new RandomFactoryBuilder()
            .setStarBaseChance(0)
            .setStarshipQuadrant(new Coord(1, 2))
            .setStarshipSector(new Coord(1, 1));

        var game = GameBuilder.getGame(random);

        var starbaseQuadrants = game.Galaxy.Quadrants.Where(q => q.HasStarBase).ToList();
        var starbaseQuadrant = Assert.Single(starbaseQuadrants);
    }
}