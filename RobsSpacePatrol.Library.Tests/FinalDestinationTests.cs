using RobsSpacePatrol.Library.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Factories;
using RobsSpacePatrol.Library.Utils;

namespace RobsSpacePatrol.CoreLogic.Tests;
public class FinalDestinationTests
{
    private readonly ITestOutputHelper output;

    public FinalDestinationTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Theory]
    [InlineData(2, 2, 2, 2, 5.0, 4.0, 0, 2, 0, 2, true)]
    [InlineData(6, 6, 6, 6, 1.0, 4.0, 7, 6, 7, 6, true)]
    [InlineData(2, 6, 2, 6, 5.0, 4.0, 0, 6, 0, 6, true)]
    [InlineData(6, 1, 6, 1, 1.0, 4.0, 7, 1, 7, 1, true)]

    [InlineData(7, 7, 7, 7, 7.0, 1.0, 7, 7, 7, 7, true)]
    [InlineData(7, 7, 7, 7, 1.0, 1.0, 7, 7, 7, 7, true)]
    [InlineData(0, 0, 0, 0, 3.0, 1.0, 0, 0, 0, 0, true)]
    [InlineData(0, 0, 0, 0, 5.0, 1.0, 0, 0, 0, 0, true)]

    [InlineData(7, 0, 7, 0, 3.0, 1.0, 7, 0, 7, 0, true)]
    [InlineData(7, 0, 7, 0, 1.0, 1.0, 7, 0, 7, 0, true)]
    [InlineData(0, 7, 0, 7, 5.0, 1.0, 0, 7, 0, 7, true)]
    [InlineData(0, 7, 0, 7, 7.0, 1.0, 0, 7, 0, 7, true)]

    [InlineData(0, 0, 0, 0, 1.0, 1.0, 0, 0, 1, 0, false)]
    [InlineData(0, 0, 0, 0, 1.0, 0.1, 1, 0, 0, 0, false)]
    [InlineData(6, 7, 7, 7, 1.0, 0.1, 7, 7, 7, 7, false)]
    public void verify_FinalDestinationReturnValue(int sx, int sy, int qx, int qy, double DIR, double DIST, int finalSX, int finalSY, int finalQX, int finalQY, bool outsideGalaxySW )
    {
        IRandomFactory rf = new RandomFactoryBuilder().GetFactories();

        var entS = new Sector(new Coord(sx, sy), new (Global.SectorStarship));
        var q = new Quadrant(new Coord(qx, qy), 0,0,false,false);
        var curQ = new CurrentQuadrantFactory().createCurrentQuadrant(q, rf, entS.Coord);

        var FDO = CalculationUtilitiesV2.getFinalDestination(DIR, DIST, entS, curQ);

        Assert.Equal(outsideGalaxySW, FDO.OutsideGalaxy);
        Assert.False(FDO.ObjectHit);
        Assert.Equal(finalSX, FDO.FinalSectorCoord!.X);
        Assert.Equal(finalSY, FDO.FinalSectorCoord!.Y);
        Assert.Equal(finalQX, FDO.FinalQuadrantCoord!.X);
        Assert.Equal(finalQY, FDO.FinalQuadrantCoord!.Y);
    }

}
