using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library.Models;
using RobsSpacePatrol.Library.Utils;

namespace RobsSpacePatrol.CoreLogic.Tests;
public class CalculationUtilsTests
{
    [Fact]
    public void test_QS2Univ()
    {
        int q = 5;
        int s = 3;

        var value = CalculationUtilitiesV2.QS2Univ(q, s);

        Assert.Equal(q * Global.MaxQuadrantsXY + s, value);
    }

    [Fact]
    public void test_Univ2Quadrant()
    {
        int univ = 16;

        var value = CalculationUtilitiesV2.Univ2QS(univ);

        Assert.Equal(univ / Global.MaxQuadrantsXY, value.quad);
    }

    [Fact]
    public void test_Univ2Sector()
    {
        int univ = 19;

        var value = CalculationUtilitiesV2.Univ2QS(univ);

        Assert.Equal(3, value.sect);
    }

    [Fact]
    public void test_Univ2QS_Negative()
    {
        var univX = CalculationUtilitiesV2.QS2Univ(-1, -5);
        var univY = CalculationUtilitiesV2.QS2Univ(-3, -7);


        var res = CalculationUtilitiesV2.Univ2QS(univX, univY);

        Assert.Equal(-1, res.QuadrantCoord.X);
        Assert.Equal(-3, res.QuadrantCoord.Y);
        Assert.Equal(-5, res.SectorCoord.X);
        Assert.Equal(-7, res.SectorCoord.Y);
    }

    [Fact]
    public void test_Univ2QS_SmallDistance()
    {
        var coord1 = new Coord(0, 0);
        var coord2 = new Coord(0, 1);


        var res = CalculationUtilitiesV2.getDistance(coord1, coord2);


        Assert.Equal(1, res);
    }

    [Fact]
    public void Distance2Energy_OneSectorMovement_CostsFourEnergy()
    {
        var result = CalculationUtilitiesV2.Distance2Energy(
            new Coord(0, 0),
            new Coord(0, 0),
            new Coord(0, 0),
            new Coord(1, 0));

        Assert.Equal(Global.Sector2EnergyConversion, result);
    }

    [Fact]
    public void Distance2Energy_CrossingQuadrantBoundaryByOneSector_CostsFourEnergy()
    {
        var result = CalculationUtilitiesV2.Distance2Energy(
            new Coord(0, 0),
            new Coord(7, 0),
            new Coord(1, 0),
            new Coord(0, 0));

        Assert.Equal(Global.Sector2EnergyConversion, result);
    }
}
