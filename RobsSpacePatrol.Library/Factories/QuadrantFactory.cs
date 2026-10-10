using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library.eNums;
using RobsSpacePatrol.Library.Factories.Interfaces;
using RobsSpacePatrol.Library.Models;

namespace RobsSpacePatrol.Library.Factories;
public class QuadrantFactory : IQuadrantFactory
{
    public Quadrant createQuadrant(Coord coord, IRandomFactory randomFactory)
    {
        var NumberOfStars = randomFactory.GetRandomInteger(eRandomIntType.StarQuantity);
        var EnemyChance = randomFactory.GetRandomInteger(eRandomIntType.EnemyChance);
        var StarbaseChance = randomFactory.GetRandomInteger(eRandomIntType.StarBaseChance);

        var TotalEnemies = 0;

        if (EnemyChance > Global.EnemyZeroOne && EnemyChance <= Global.EnemyOneTwo)
        {
            TotalEnemies = 1;
        }
        else if (EnemyChance > Global.EnemyOneTwo && EnemyChance <= Global.EnemyTwoThree)
        {
            TotalEnemies = 2;
        }
        else if (EnemyChance > Global.EnemyTwoThree)
        {
            TotalEnemies = 3;
        }

        var StarBasePresent = false;
        if (StarbaseChance > Global.StarbaseChance)
        {
            StarBasePresent = true;
        }
        var newQuadrant = new Quadrant(coord, TotalEnemies, NumberOfStars, false, StarBasePresent);

        return newQuadrant;
    }
}
