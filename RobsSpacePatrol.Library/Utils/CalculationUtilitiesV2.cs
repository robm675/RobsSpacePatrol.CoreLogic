using NLog.Targets;
using System.Collections.Generic;
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library.Models;

namespace RobsSpacePatrol.Library.Utils;

public class CalculationUtilitiesV2
{
    public static int QS2Univ(int inQuadrant, int inSector)
    {
        int x = inQuadrant * 8 + inSector;
        return x;
    }
    public static (int quad, int sect) Univ2QS(int UniversalCoord)
    {
        int quad = ((int)Math.Round((double)UniversalCoord)) / 8;
        //int sect = ((int)Math.Round((double)UniversalCoord)) % 8;
        int sect = (int)((double)UniversalCoord % 8);
        return (quad, sect);
    }
    public static (Coord QuadrantCoord, Coord SectorCoord) Univ2QS(int UniversalCoordX, int UniversalCoordY)
    {
        (int QX, int SX) = Univ2QS(UniversalCoordX);
        (int QY, int SY) = Univ2QS(UniversalCoordY);
        return (new Coord(QX, QY), new Coord(SX, SY));
    }
    public static double getDistance(Coord coord1, Coord coord2)
    {
        double x = (double)coord2.X - (double)coord1.X;
        double y = (double)coord2.Y - (double)coord1.Y;
        return Math.Sqrt(x * x + y * y);
    }
    public static double getDirection(Coord coord1, Coord coord2)
    {
        double direction = 0;
        if (coord1.X == coord2.X)
        {
            if (coord1.Y < coord2.Y)
            {
                direction = 7;
            }
            else
            {
                direction = 3;
            }
        }
        else if (coord1.Y == coord2.Y)
        {
            if (coord1.X < coord2.X)
            {
                direction = 1;
            }
            else
            {
                direction = 5;
            }
        }
        else
        {
            double dy = Math.Abs(coord2.Y - coord1.Y);
            double dx = Math.Abs(coord2.X - coord1.X);
            double angle = Math.Atan2(dy, dx);
            if (coord1.X < coord2.X)
            {
                if (coord1.Y < coord2.Y)
                {
                    direction = 9.0 - 4.0 * angle / Math.PI;
                }
                else
                {
                    direction = 1.0 + 4.0 * angle / Math.PI;
                }
            }
            else
            {
                if (coord1.Y < coord2.Y)
                {
                    direction = 5.0 + 4.0 * angle / Math.PI;
                }
                else
                {
                    direction = 5.0 - 4.0 * angle / Math.PI;
                }
            }
        }
        return direction;
    }
    public static int EnemyShieldHit(int NumberOfEnemies, int EnergyOfShot, double DistanceToEnemy, int EnergyFactorToReducePerSector)
    {
        int LaserAmountPerEnemy = (int)Math.Round((double)EnergyOfShot / NumberOfEnemies, 0);
        var LaserEnergyToReduceDueToDistance = (int)(DistanceToEnemy * EnergyFactorToReducePerSector);
        var TotalEnergyHitOnEnemyShields = (int)(LaserAmountPerEnemy - LaserEnergyToReduceDueToDistance);
        if (TotalEnergyHitOnEnemyShields < 0)
        {
            TotalEnergyHitOnEnemyShields = 0;
        }
        return TotalEnergyHitOnEnemyShields;
    }
    public static int Distance2Energy(double inDistance)
    {
        return (int)Math.Round(inDistance * Global.Sector2EnergyConversion);
    }
    private static int Distance2Energy(Coord Start, Coord End)
    {
        double DIST = getDistance(Start, End);
        return Distance2Energy(DIST) <= 0 ? 1 : Distance2Energy(DIST);
    }
    public static int Distance2Energy(Coord StartQuadrantCoord, Coord StartSectorCoord, Coord FinalQuadrantCoord, Coord FinalSectorCoord)
    {
        int startUX = QS2Univ(StartQuadrantCoord.X, StartSectorCoord.X);
        int startUY = QS2Univ(StartQuadrantCoord.Y, StartSectorCoord.Y);
        int finalUX = QS2Univ(FinalQuadrantCoord.X, FinalSectorCoord.X);
        int finalUY = QS2Univ(FinalQuadrantCoord.Y, FinalSectorCoord.Y);
        return Distance2Energy(new Coord(startUX, startUY), new Coord(finalUX, finalUY));
    }
    public static (Coord TopLeftCoord, Coord BottomRightCoord) getCoordRange(Coord CenterCoord)
    {
        var leftX = CenterCoord.X - 1 < 0 ? 0 : CenterCoord.X - 1;
        var rightX = CenterCoord.X + 1 > 7 ? 7 : CenterCoord.X + 1;
        var topY = CenterCoord.Y - 1 < 0 ? 0 : CenterCoord.Y - 1;
        var bottomY = CenterCoord.Y + 1 > 7 ? 7 : CenterCoord.Y + 1;

        return (new Coord(leftX, topY), new Coord(rightX, bottomY));
    }
    public static double Distance2Time(double inDistance)
    {
        return inDistance * Global.Sector2TimeConversion;
    }
    public static FinalDestination getFinalDestination(double DIR, double DIST, Sector starshipSector, CurrentQuadrant currentQuadrant)
    {
        int EntSX = starshipSector.Coord.X;
        int EntSY = starshipSector.Coord.Y;
        int EntQX = currentQuadrant.Coord!.X;
        int EntQY = currentQuadrant.Coord!.Y;

        double estFinalUniX = CalculationUtilitiesV2.QS2Univ(EntQX, EntSX);
        double estFinalUniY = CalculationUtilitiesV2.QS2Univ(EntQY, EntSY);

        //Convert quad sect into universal coord
        double distUniv = DIST * 8;

        WriteOutput($"distUniv={distUniv}");

        //calculate angle
        double angle = -(Math.PI * (DIR - 1.0) / 4.0);

        //calculate distance
        double dx = Math.Round(distUniv * Math.Cos(angle), 5);
        double dy = Math.Round(distUniv * Math.Sin(angle), 5);

        WriteOutput($"dx={dx}    dy={dy}");

        double vx = dx / 1000;
        double vy = dy / 1000;

        WriteOutput($"vx={vx}    vy={vy}");

        int lastGoodSX = -1;
        int lastGoodSY = -1;
        int lastGoodQX = -1;
        int lastGoodQY = -1;

        for (int i = 0; i < 1000; i++)
        {
            estFinalUniX += vx;
            estFinalUniY += vy;

            WriteOutput($"estFinalUniX = {estFinalUniX}");
            WriteOutput($"estFinalUniY = {estFinalUniY}");


            (int tempQX, int tempSX) = CalculationUtilitiesV2.Univ2QS((int)Math.Round(estFinalUniX));
            (int tempQY, int tempSY) = CalculationUtilitiesV2.Univ2QS((int)Math.Round(estFinalUniY));

            WriteOutput($"tempQX = {tempQX} tempQY = {tempQY} tempSX = {tempSX} tempSY  {tempSY}");

            estFinalUniX = Math.Round(estFinalUniX, 5);
            estFinalUniY = Math.Round(estFinalUniY, 5);



            //outside galaxy
            if (tempQX < 0 || tempQX > Global.MaxQuadrantsXY - 1 || tempQY < 0 || tempQY > Global.MaxQuadrantsXY - 1)
                return new(new Coord(lastGoodQX, lastGoodQY), new Coord(lastGoodSX, lastGoodSY), true, false);

            //outside galaxy
            if (tempSX < 0 || tempSX > Global.MaxSectorsXY - 1 || tempSY < 0 || tempSY > Global.MaxSectorsXY - 1)
                return new(new Coord(lastGoodQX, lastGoodQY), new Coord(lastGoodSX, lastGoodSY), true, false);

            if (tempQX == EntQX && tempQY == EntQY)
            {
                var targetSector = new Coord(tempSX, tempSY);
                var curSector = currentQuadrant.Sectors!.First(z => z.Coord == targetSector);
                //ran into something
                if (curSector.SectorContents != Global.SectorEmpty &&
                    curSector.SectorContents != Global.SectorStarship)
                    return new(currentQuadrant.Coord, new Coord(lastGoodSX, lastGoodSY), false, true);
            }
            lastGoodSX = tempSX;
            lastGoodSY = tempSY;
            lastGoodQX = tempQX;
            lastGoodQY = tempQY;
        }

        //WriteOutput($"estFinalUniX = {estFinalUniX}");
        //WriteOutput($"estFinalUniY = {estFinalUniY}");

        var univX_Int = (int)Math.Round(estFinalUniX);
        var univY_Int = (int)Math.Round(estFinalUniY);

        //WriteOutput($"univX_Int = {univX_Int}");
        //WriteOutput($"univY_Int = {univY_Int}");

        (Coord FinalQuadrant, Coord FinalSector) = CalculationUtilitiesV2.Univ2QS(univX_Int, univY_Int);

        //Console.WriteLine($"FinalQuadrant: {FinalQuadrant.ToString()}");
        //Console.WriteLine($"FinalSector: {FinalSector.ToString()}");

        if (estFinalUniX < 0 || estFinalUniX >= Global.MaxQuadrantsXY * Global.MaxSectorsXY || estFinalUniY < 0 || estFinalUniY >= Global.MaxQuadrantsXY * Global.MaxSectorsXY)
        {
            //Console.WriteLine($"estFinalUniX : {estFinalUniX} estFinalUniY = {estFinalUniY}");
            return new(new Coord(-1, -1), new Coord(-1, -1), false, false);
        }

        return new(FinalQuadrant, FinalSector, false, false);
    }
    public static (string? ObjectHit, double? targetDIST, Coord FinalCoord, List<Coord>? TrackingCoords) detectObject(CurrentQuadrant currentQuadrant,
       Coord StartSectorCoord, bool isNavigation, double DIR, double DIST = 99)
    {
        if (currentQuadrant.Sectors == null)
            throw new Exception("CheckForObjectV2.detectObject: CurrentQuadrant.Sectors is null!");

        var TrackingCoords = new List<Coord>();

        //Convert quad sect into universal coord
        double dist = DIST * 8;

        //calculate angle
        double angle = -(Math.PI * (DIR - 1.0) / 4.0);

        //calculate distance
        double dx = dist * Math.Cos(angle);
        double dy = dist * Math.Sin(angle);

        //calculate vector, this is how much the starship will move with each iteration in the loop below
        double vx = dx / 1000;
        double vy = dy / 1000;

        //set target universal coord
        double targetX = StartSectorCoord.X;
        double targetY = StartSectorCoord.Y;

        //set last sector that was a valid one in the current quadrant
        //  for use in the event we run into something
        int lastGoodSectX = StartSectorCoord.X;
        int lastGoodSectY = StartSectorCoord.Y;

        int LastTrackingX = lastGoodSectX;
        int LastTrackingY = lastGoodSectY;
        TrackingCoords.Add(new Coord(LastTrackingX, LastTrackingY));

        //loop thru each vector change to check for things in the way as we move through the current quadrant
        for (int i = 0; i < 1000; i++)
        {
            //set the sector moved to
            targetX += vx;
            targetY += vy;

            int tempSectX = (int)Math.Round(targetX);
            int tempSectY = (int)Math.Round(targetY);

            if (tempSectX != LastTrackingX || tempSectY != LastTrackingY)
            {
                LastTrackingX = lastGoodSectX;
                LastTrackingY = lastGoodSectY;
                var tempCoord = new Coord(LastTrackingX, LastTrackingY);
                if (!TrackingCoords.Any(z => z.X == LastTrackingX && z.Y == LastTrackingY))
                    TrackingCoords.Add(new Coord(LastTrackingX, LastTrackingY));
            }

            //Have we moved outside the current quadrant?
            if (tempSectX < 0 || tempSectY < 0 || tempSectX > 7 || tempSectY > 7)
            {
                break;
            }

            //get temporary target sector for anything.
            var targetCoord = new Coord(tempSectX, tempSectY);
            var tempSector = currentQuadrant.Sectors.First(z => z.Coord == targetCoord);

            //if we run into something abort moving, and exit
            if (tempSector.SectorContents != Global.SectorEmpty &&
                tempSector.SectorContents != Global.SectorStarship)
            {
                //get Coord of last location
                var FinalCoord = new Coord(tempSectX, tempSectY);
                if (isNavigation)
                {
                    int TrackingCoordIDX = TrackingCoords.Count - 1;
                    FinalCoord = new Coord(TrackingCoords[TrackingCoordIDX].X, TrackingCoords[TrackingCoordIDX].Y);
                    var noDistanceMoved = CalculationUtilitiesV2.getDistance(StartSectorCoord, FinalCoord);
                    while (currentQuadrant.Sectors.First(z => z.Coord == FinalCoord).SectorContents != Global.SectorEmpty)
                    {
                        FinalCoord = new Coord(TrackingCoords[TrackingCoordIDX].X, TrackingCoords[TrackingCoordIDX].Y);
                        TrackingCoordIDX--;

                        //didn't move any, so there's no tracking
                        if (TrackingCoordIDX < 0)
                            return (tempSector.SectorContents, noDistanceMoved, StartSectorCoord, TrackingCoords);
                    }
                }

                //adding last location to tracking
                if (!TrackingCoords.Any(z => z.X == FinalCoord.X && z.Y == FinalCoord.Y))
                    TrackingCoords.Add(FinalCoord);

                //calculate distance to aborted coord
                var DistanceMoved = CalculationUtilitiesV2.getDistance(StartSectorCoord, FinalCoord);

                if (isNavigation && currentQuadrant.Sectors.First(z => z.Coord == FinalCoord).SectorContents != Global.SectorEmpty)
                    throw new Exception("CheckForObjectV2.detectObject: FinalCoord is NOT empty!");


                //What was run into
                var ObjectHit = tempSector.SectorContents;

                return (ObjectHit, DistanceMoved, FinalCoord, TrackingCoords);
            }

            //set for next loop
            lastGoodSectX = tempSectX;
            lastGoodSectY = tempSectY;
        }

        return (Global.SectorEmpty, -1, new Coord(-1, -1), TrackingCoords);
    }


    public static QS GetDistanceFromDecimal(double value)
    {
        int tenths = ((int)(value * 10)) % 10;

        int col2 = tenths % 8;

        int col3 = (int)value + (tenths / 8);
        return new(col2, col3);
    }

    public static void WriteOutput(string value)
    {
        //Console.WriteLine(value);
    }

    // scans the current quarant starting at specified sector coord in the given direction for an object
    // if the scan goes outside the current quadrant, we're done - return DetectedObject with empty object hit
    // when object is detected
    //   if nav -> return tracking coords - NOT including the one with the object, final coord is the last empty sector coord
    //                  the detected object is returned.
    //   if NOT nav -> return tracking coords - INCLUDING the one with the object, final coord is the sector coord containing object detected
    public static DetectedObject ScanForObject(CurrentQuadrant currentQuadrant, Coord startSectorCoord, double dir, bool isNav, double dist = 99.0)
    {
        if (isNav && Math.Round(dist, 1) == 99.0)
            throw new Exception("Invalid in put in ScanForObject - isNav && dist was not given");

        if (!isNav && Math.Round(dist, 1) != 99.0)
            throw new Exception("Invalid in put in ScanForObject - !isNav && dist was given");


        var TrackingCoords = new List<Coord>();

        //Convert quad sect into universal coord
        var maxDistQS = GetDistanceFromDecimal(dist);
        //double maxDist = dist * 8;

        //calculate angle
        double angle = -(Math.PI * (dir - 1.0) / 4.0);

        //calculate distance
        double dx = Global.MaxQuadrantsXY * Math.Cos(angle);
        double dy = Global.MaxQuadrantsXY * Math.Sin(angle);

        //calculate vector, this is how much the starship will move with each iteration in the loop below
        double vx = dx / 1000;
        double vy = dy / 1000;

        //set target universal coord
        double targetX = startSectorCoord.X;
        double targetY = startSectorCoord.Y;

        for (int i = 0; i < 1000; i++)
        {
            //set the sector being checked by adding the delta values
            targetX += vx;
            targetY += vy;

            int tempSectX = (int)targetX;
            int tempSectY = (int)targetY;

            //Have we moved outside the current quadrant?
            if (tempSectX < 0 || tempSectY < 0 || tempSectX >= Global.MaxQuadrantsXY || tempSectY >= Global.MaxQuadrantsXY)
                return new DetectedObject(Global.SectorEmpty);  //if so, we're done

            var curSectorCoord = new Coord(tempSectX, tempSectY);
            if (TrackingCoords.Any(z => z == curSectorCoord)) //if we checked this sector coord before, don't go any further
                continue;

            //if we're still pointing to the sector containing the Starship, skip it
            if (currentQuadrant.GetSectorByCoord(curSectorCoord).HasStarship) continue;


            WriteOutput($"cursector=[{curSectorCoord}]");

            var testDist = getDistance(startSectorCoord, curSectorCoord);
            WriteOutput($"testDist={testDist}");

            var testDistDecimal = testDist / Global.MaxQuadrantsXY;
            var testDistQS = GetDistanceFromDecimal(testDistDecimal);

            WriteOutput($"testDistDecimal={testDistDecimal}");
            WriteOutput($"testDistQS={testDistQS} maxDistQS={maxDistQS}");

            if (testDistQS > maxDistQS && isNav) //we've passed the max dist - if dist is given then we MUST be navigating
            {
                var lastTrackingCoord = TrackingCoords.Last();
                testDist = getDistance(startSectorCoord, lastTrackingCoord);
                testDistDecimal = Math.Round(testDist / Global.MaxQuadrantsXY, 1);
                TrackingCoords.Remove(TrackingCoords.Last());
                return new(Global.SectorEmpty, testDistDecimal, lastTrackingCoord, TrackingCoords);
            }


            if (!currentQuadrant.GetSectorByCoord(curSectorCoord).IsEmpty)  //we've hit object, we're done
            {
                var sectorContentsDetected = currentQuadrant.GetSectorByCoord(curSectorCoord).SectorContents;

                if (isNav)
                {
                    //if we ARE navigating, then it's possible to have NO traveling distance
                    if (TrackingCoords.Count == 0) //we didn't move, because there aren't any tracking coords
                        return new DetectedObject(sectorContentsDetected, 0, startSectorCoord, new()); // return contents, no distance, starting sect coord, no tracking


                    var lastTrackingCoord = TrackingCoords.Last();
                    //TrackingCoords.Remove(TrackingCoords.Last());
                    var traveledDist = getDistance(startSectorCoord, lastTrackingCoord);
                    return new(sectorContentsDetected, traveledDist, lastTrackingCoord, TrackingCoords);
                }
                else
                {
                    var traveledDist = getDistance(startSectorCoord, curSectorCoord);
                    if (!TrackingCoords.Any(z => z == curSectorCoord))
                        TrackingCoords.Add(curSectorCoord);
                    return new DetectedObject(sectorContentsDetected, traveledDist, TrackingCoords.Last(), TrackingCoords);
                }
            }
            else
                if (!TrackingCoords.Any(z => z == curSectorCoord))  //check to see if the current sector coord have been recorded, if not, add them to the list
                TrackingCoords.Add(curSectorCoord);
            WriteOutput("----");
        }
        throw new Exception("Got here and shouldn't have!");
    }
}
