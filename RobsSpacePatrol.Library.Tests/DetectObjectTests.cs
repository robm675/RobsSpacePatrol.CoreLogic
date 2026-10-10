
using Newtonsoft.Json.Linq;
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Factories;
using RobsSpacePatrol.Library.Utils;

namespace RobsSpacePatrol.CoreLogic.Tests;

public class DetectObjectTests
{
    [Fact]
    public void test_DetectObjectBasic_OneSector()
    {
        int sx = 4;
        int sy = 3;
        int qx = 0;
        int qy = 0;
        int starX = 5;
        int starY = 3;

        var starshipSector = new Sector(new Coord(sx, sy), Global.SectorStarship);
        var quad = new Quadrant(new Coord(qx, qy), 0, 0, false, false);
        var rf = new RandomFactoryBuilder().GetFactories();
        var curQuad = new CurrentQuadrantFactory().createCurrentQuadrant(quad, rf, starshipSector.Coord);
        curQuad.Sectors!.First(z => z.Coord.X == starX && z.Coord.Y == starY).SectorContents = Global.SectorStar;

        var res = CalculationUtilitiesV2.ScanForObject(curQuad, starshipSector.Coord, 1, true, .3);
        //var res = CalculationUtilitiesV2.detectObject(curQuad, starshipSector.Coord, true, 1, .3);

        Assert.Equal(Global.SectorStar, res.ObjectHit);
        Assert.Equal(sx, res.FinalCoord!.X);
        Assert.Equal(sy, res.FinalCoord!.Y);
    }
    [Fact]
    public void test_DetectObjectBasic_OneQuadrant()
    {
        int sx = 1;
        int sy = 3;
        int qx = 0;
        int qy = 0;
        int starX = 7;
        int starY = 3;

        var starshipSector = new Sector(new Coord(sx, sy), Global.SectorStarship);
        var quad = new Quadrant(new Coord(qx, qy), 0, 0, false, false);
        var rf = new RandomFactoryBuilder().GetFactories();
        var curQuad = new CurrentQuadrantFactory().createCurrentQuadrant(quad, rf, starshipSector.Coord);
        curQuad.Sectors!.First(z => z.Coord.X == starX && z.Coord.Y == starY).SectorContents = Global.SectorStar;

        var res = CalculationUtilitiesV2.ScanForObject(curQuad, starshipSector.Coord, 1, true, 1);
        //var res = CalculationUtilitiesV2.detectObject(curQuad, starshipSector.Coord, true, 1, 1);

        Assert.Equal(Global.SectorStar, res.ObjectHit);
        Assert.Equal(6, res.FinalCoord!.X);
        Assert.Equal(sy, res.FinalCoord!.Y);
    }

    [Theory]
    [InlineData(0, 0, 3)]
    [InlineData(0, 0, 5)]
    [InlineData(7, 7, 7)]
    [InlineData(7, 7, 1)]
    public void test_DetectObject_LeaveCQ(int startSX, int startSY, double dir)
    {
        var starshipSector = new Sector(new Coord(startSX, startSY), Global.SectorStarship);
        var quad = new Quadrant(new Coord(0, 0), 0, 0, false, false);
        var rf = new RandomFactoryBuilder().GetFactories();
        var curQuad = new CurrentQuadrantFactory().createCurrentQuadrant(quad, rf, starshipSector.Coord);


        DetectedObject result = CalculationUtilitiesV2.ScanForObject(curQuad, starshipSector.Coord, dir, false);

        Assert.Equal(Global.SectorEmpty, result.ObjectHit);
        Assert.Null(result.FinalCoord);
        Assert.Null(result.targetDIST);
        Assert.Null(result.TrackingCoords);
    }

    [Theory]
    [InlineData(4, 4, 3)]
    [InlineData(4, 4, 5)]
    [InlineData(4, 4, 7)]
    [InlineData(4, 4, 1)]
    public void test_DetectObject_HitObject_NoDistTraveled(int startSX, int startSY, double dir)
    {
        var starshipSector = new Sector(new Coord(startSX, startSY), Global.SectorStarship);
        var quad = new Quadrant(new Coord(0, 0), 0, 0, false, false);
        var rf = new RandomFactoryBuilder().GetFactories();
        var curQuad = new CurrentQuadrantFactory().createCurrentQuadrant(quad, rf, starshipSector.Coord);
        curQuad.GetSectorByCoord(new(4, 5)).SetStar();
        curQuad.GetSectorByCoord(new(4, 3)).SetStar();
        curQuad.GetSectorByCoord(new(5, 4)).SetStar();
        curQuad.GetSectorByCoord(new(3, 4)).SetStar();



        DetectedObject result = CalculationUtilitiesV2.ScanForObject(curQuad, starshipSector.Coord, dir, true, 2);

        Assert.Equal(Global.SectorStar, result.ObjectHit);
        Assert.Equal(new(4, 4), result.FinalCoord);
        Assert.Equal(0, result.targetDIST);
        Assert.Empty(result.TrackingCoords!);
    }


    [Theory]
    [InlineData(4, 4, 3, 4, 3, 4, 2)]  // up y decreases
    [InlineData(4, 4, 7, 4, 5, 4, 6)] //down y increases
    [InlineData(4, 4, 5, 3, 4, 2, 4)] //left x decreases
    [InlineData(4, 4, 1, 5, 4, 6, 4)] //right x increases
    public void test_DetectObject_HitObject_OneSectorTraveled_Space(int startSX, int startSY, double dir, int finalX, int finalY, int starX, int starY)
    {
        var starshipSector = new Sector(new Coord(startSX, startSY), Global.SectorStarship);
        var quad = new Quadrant(new Coord(0, 0), 0, 0, false, false);
        var rf = new RandomFactoryBuilder().GetFactories();
        var curQuad = new CurrentQuadrantFactory().createCurrentQuadrant(quad, rf, starshipSector.Coord);
        curQuad.GetSectorByCoord(new(starX, starY)).SetStar();



        DetectedObject result = CalculationUtilitiesV2.ScanForObject(curQuad, starshipSector.Coord, dir, true, 2);

        Assert.Equal(Global.SectorStar, result.ObjectHit);
        Assert.Equal(new(finalX, finalY), result.FinalCoord);
        Assert.Equal(1, result.targetDIST);
    }

    [Theory]
    [InlineData(4, 4, 3, 4, 3, 4, 2)]  // up y decreases
    [InlineData(4, 4, 7, 4, 5, 4, 6)] //down y increases
    [InlineData(4, 4, 5, 3, 4, 2, 4)] //left x decreases
    [InlineData(4, 4, 1, 5, 4, 6, 4)] //right x increases
    public void test_DetectObject_HitObject_OneSectorTraveled_Impulse(int startSX, int startSY, double dir, int finalX, int finalY, int starX, int starY)
    {
        var starshipSector = new Sector(new Coord(startSX, startSY), Global.SectorStarship);
        var quad = new Quadrant(new Coord(0, 0), 0, 0, false, false);
        var rf = new RandomFactoryBuilder().GetFactories();
        var curQuad = new CurrentQuadrantFactory().createCurrentQuadrant(quad, rf, starshipSector.Coord);
        curQuad.GetSectorByCoord(new(starX, starY)).SetStar();



        DetectedObject result = CalculationUtilitiesV2.ScanForObject(curQuad, starshipSector.Coord, dir, true, 0.5);

        Assert.Equal(Global.SectorStar, result.ObjectHit);
        Assert.Equal(new(finalX, finalY), result.FinalCoord);
        Assert.Equal(1, result.targetDIST);
    }

    [Theory]
    [InlineData(4, 4, 3, 4, 3, 4, 2)]  // up y decreases
    [InlineData(4, 4, 7, 4, 5, 4, 6)] //down y increases
    [InlineData(4, 4, 5, 3, 4, 2, 4)] //left x decreases
    [InlineData(4, 4, 1, 5, 4, 6, 4)] //right x increases
    public void test_DetectObject_HitObject_OneSectorTraveled_SetDist(int startSX, int startSY, double dir, int finalX, int finalY, int starX, int starY)
    {
        var starshipSector = new Sector(new Coord(startSX, startSY), Global.SectorStarship);
        var quad = new Quadrant(new Coord(0, 0), 0, 0, false, false);
        var rf = new RandomFactoryBuilder().GetFactories();
        var curQuad = new CurrentQuadrantFactory().createCurrentQuadrant(quad, rf, starshipSector.Coord);
        curQuad.GetSectorByCoord(new(starX, starY)).SetStar();



        DetectedObject result = CalculationUtilitiesV2.ScanForObject(curQuad, starshipSector.Coord, dir, true, 0.1);


        Console.WriteLine($"startSX={startSX} startSY={startSY} dir={dir.ToString("0")} finalX={finalX} finalY={finalY} starX={starX} starY={starY}");

        Assert.Equal(Global.SectorEmpty, result.ObjectHit);
        Assert.Equal(new(finalX, finalY), result.FinalCoord);
        Assert.Equal(0.1, result.targetDIST);
    }

    #region test_DistanceToDecimal
    [Theory]
    [InlineData(0.1, 1, 0)]
    [InlineData(0.2, 2, 0)]
    [InlineData(0.3, 3, 0)]
    [InlineData(0.4, 4, 0)]
    [InlineData(0.5, 5, 0)]
    [InlineData(0.6, 6, 0)]
    [InlineData(0.7, 7, 0)]
    [InlineData(0.8, 0, 1)]
    [InlineData(0.9, 1, 1)]
    [InlineData(1.0, 0, 1)]
    [InlineData(1.1, 1, 1)]
    [InlineData(1.2, 2, 1)]
    [InlineData(1.3, 3, 1)]
    [InlineData(1.4, 4, 1)]
    [InlineData(1.5, 5, 1)]
    [InlineData(1.6, 6, 1)]
    [InlineData(1.7, 7, 1)]
    [InlineData(1.8, 0, 2)]
    [InlineData(1.9, 1, 2)]
    [InlineData(2.0, 0, 2)]
    [InlineData(2.1, 1, 2)]
    [InlineData(2.2, 2, 2)]
    public void test_DistanceToDecimal(double value, int expectedSector, int expectedQuadrant)
    {
        var data = CalculationUtilitiesV2.GetDistanceFromDecimal(value);

        Console.WriteLine($"value={value.ToString("0.0")} s={expectedSector} q={expectedQuadrant}");

        Assert.Equal(expectedSector, data.S);
        Assert.Equal(expectedQuadrant, data.Q);
    }
    #endregion
}
