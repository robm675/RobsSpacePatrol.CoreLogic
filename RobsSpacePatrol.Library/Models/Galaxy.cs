using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library.eNums;
using RobsSpacePatrol.Library.Factories.Interfaces;

namespace RobsSpacePatrol.Library.Models;

public interface IGalaxy
{
    CurrentQuadrant CurrentQuadrant { get; set; }
    Starship Starship { get; set; }
    List<Quadrant> Quadrants { get; set; }
    double MissionTime { get; set; }
    double MissionTimeDeadLine { get; set; }
    bool skipEnemyMoveSW { get; }
    bool forceEnemyMoveSW { get; }
    bool stopEnemyFireSW { get; }


    eNavMessage CheckRouteAndMoveStarship(IRandomFactory randomFactory, ICurrentQuadrantFactory currentQuadrantFactory, double DIR, double DIST, int EstimateAmountOfEnergyForTrip, Coord StartSectorCoord, Coord FinalQuadrant, Coord FinalSector);
    bool Equals(Galaxy? other);
    bool Equals(object? obj);
    int getEnemiesRemaining();
    string getGalaxyFormatted();
    int GetHashCode();
    Quadrant? GetQuadrantByCoord(Coord coord);
    QuadrantDTO? GetQuadrantByCoordDTO(CoordDTO coord);
    void MoveStarshipQuadrant(ICurrentQuadrantFactory currentQuadrantFactory, IRandomFactory randomFactory, Quadrant newQuadrant, Coord sectorCoord);
    string ToString();
    void debugSet(bool skipEnemyMove = false, bool forceEnemyMove = false, bool stopEnemyFire = false);

    List<QuadrantDTO> toQuadrantDTO { get; }
}

public record Galaxy : IGalaxy
{
    public Galaxy(Starship starship, CurrentQuadrant currentQuadrant, List<Quadrant> quadrants, double missionTime)
    {
        Starship = starship;
        CurrentQuadrant = currentQuadrant;
        Quadrants = quadrants;
        MissionTime = missionTime;
        MissionTimeDeadLine = missionTime + (Quadrants.Sum(z => z.NumberOfEnemies) * Global.MissionTimeUnitsPerEnemyRatio);
    }
    [JsonConstructor]
    public Galaxy(Starship starship, CurrentQuadrant currentQuadrant, List<Quadrant> quadrants, double missionTime, double missionTimeDeadLine, bool skipEnemyMoveSW, bool forceEnemyMoveSW, bool stopEnemyFireSW)
    {
        Starship = starship;
        CurrentQuadrant = currentQuadrant;
        Quadrants = quadrants;
        MissionTime = missionTime;
        MissionTimeDeadLine = missionTimeDeadLine;
        this.skipEnemyMoveSW = skipEnemyMoveSW;
        this.forceEnemyMoveSW = forceEnemyMoveSW;
        this.stopEnemyFireSW = stopEnemyFireSW;
    }
    public Starship Starship { get; set; }
    public CurrentQuadrant CurrentQuadrant { get; set; }
    public List<Quadrant> Quadrants { get; set; }
    public double MissionTime { get; set; }
    public double MissionTimeDeadLine { get; set; }
    public bool skipEnemyMoveSW { get; private set; } = false;
    public bool forceEnemyMoveSW { get; private set; } = false;
    public bool stopEnemyFireSW { get; private set; } = false;
    public void debugSet(bool skipEnemyMove = false, bool forceEnemyMove = false, bool stopEnemyFire = false)
    {
        this.skipEnemyMoveSW = skipEnemyMove;
        this.forceEnemyMoveSW = forceEnemyMove;
        this.stopEnemyFireSW = stopEnemyFire;
    }


    public int getEnemiesRemaining()
    {
        return Quadrants.Sum(z => z.NumberOfEnemies);
    }
    public Quadrant? GetQuadrantByCoord(Coord coord)
    {
        return Quadrants.FirstOrDefault(z => z.Coord == coord);
    }
    public QuadrantDTO? GetQuadrantByCoordDTO(CoordDTO coord)
    {
        return Quadrants.FirstOrDefault(z => z.Coord.isSame(coord))?.toDTO;
    }
    public string getGalaxyFormatted()
    {
        string outstr = "";
        //                         0123456789012345678901   0123456789012345678901
        const string XHeader = "  <---------------------- X ---------------------->\n";
        const string Line = "+-----+-----+-----+-----+-----+-----+-----+-----+\n";

        outstr += XHeader;
        for (int y = 0; y <= 7; y++)
        {
            switch (y)
            {
                case 0:
                    outstr += "^";
                    break;
                case 4:
                    outstr += "Y";
                    break;
                default:
                    outstr += "|";
                    break;
            }
            outstr += " " + Line;
            outstr += "| ";
            for (int x = 0; x <= 7; x++)
            {
                var QUAD = GetQuadrantByCoord(new Coord(x, y));
                if (QUAD == null) continue;
                if (CurrentQuadrant.Coord == new Coord(x, y))
                {
                    outstr += QUAD.getQuadrantForGalaxyFormatted(true);
                }
                else
                {
                    outstr += QUAD.getQuadrantForGalaxyFormatted(false);
                }
            }
            outstr += "|\n";
        }
        outstr += "v " + Line;
        return outstr;
    }

    public eNavMessage CheckRouteAndMoveStarship(IRandomFactory randomFactory, ICurrentQuadrantFactory currentQuadrantFactory, double DIR, double DIST, int EstimateAmountOfEnergyForTrip, Coord StartSectorCoord, Coord FinalQuadrant, Coord FinalSector)
    {
        var CFO = CalculationUtilitiesV2.ScanForObject(CurrentQuadrant, StartSectorCoord, DIR, true, DIST);
        //var CFO = CalculationUtilitiesV2.detectObject(CurrentQuadrant, StartSectorCoord, true, DIR, DIST);

        var curSector = CurrentQuadrant.GetSectorByContents(Global.SectorStarship);
        if (curSector == null)
            return eNavMessage.InternalError;

        if (CFO.ObjectHit == Global.SectorEmpty)
        {
            if (FinalQuadrant.X == CurrentQuadrant.Coord.X && FinalQuadrant.Y == CurrentQuadrant.Coord.Y)
            {
                CurrentQuadrant.moveObjectInsideQuadrant(curSector.Coord, FinalSector);
            }
            else
            {
                var newQuadrant = GetQuadrantByCoord(FinalQuadrant);
                if (newQuadrant == null)
                    return eNavMessage.InternalError;
                MoveStarshipQuadrant(currentQuadrantFactory, randomFactory, newQuadrant, FinalSector);
            }

            Starship.EnergyLevel -= EstimateAmountOfEnergyForTrip;
            return eNavMessage.TransitComplete;
        }
        else
        {
            Coord NextToLastCoord;
            try
            {
                NextToLastCoord = CFO.TrackingCoords!.Last();

            }
            catch (Exception)
            {
                Console.WriteLine($"DIR={DIR}");
                Console.WriteLine($"StartSectorCoord={StartSectorCoord}");
                Console.WriteLine($"CFO.ObjectHit={CFO.ObjectHit}");

                Console.WriteLine(CurrentQuadrant.getFormatted());

                throw;
            }
            //get next to last of the tracking coords because we don't want the actual sector the object is in, but the one before



            //change the estimate because the trip was cut short
            EstimateAmountOfEnergyForTrip = CalculationUtilitiesV2.Distance2Energy(
                            CurrentQuadrant.Coord,
                            curSector.Coord,
                            CurrentQuadrant.Coord,
                            NextToLastCoord);


            CurrentQuadrant.moveObjectInsideQuadrant(curSector.Coord, NextToLastCoord);

            Starship.EnergyLevel -= EstimateAmountOfEnergyForTrip;
            return eNavMessage.BadInput_ObjectHit;
        }
    }

    public void MoveStarshipQuadrant(ICurrentQuadrantFactory currentQuadrantFactory, IRandomFactory randomFactory, Quadrant newQuadrant, Coord sectorCoord)
    {
        //Console.WriteLine(new String('-', 80));
        //Console.WriteLine($"MoveStarshipQuadrant: newQuadrant.Coord = {getCoord(newQuadrant.Coord!)}");
        //Console.WriteLine($"MoveStarshipQuadrant: sectorCoord = {getCoord(sectorCoord)}");

        CurrentQuadrant = currentQuadrantFactory.createCurrentQuadrant(newQuadrant, randomFactory, sectorCoord);

        var numStarsCQ = CurrentQuadrant.Sectors!.Count(z => z.SectorContents == Global.SectorStar);
        var q = Quadrants!.First(z => z.Coord == CurrentQuadrant.Coord);

        if (numStarsCQ != q.NumberOfStars)
        {
            Debug.WriteLine("");
            throw new Exception("Number of stars not matching");
        }


        //Console.WriteLine("MoveStarshipQuadrant: StarQuantity: " + numStarsCQ + "=" + q.NumberOfStars);
        //Console.WriteLine("MoveStarshipQuadrant: CurrentQuadrant Coords: " + getCoord(Galaxy.CurrentQuadrant.Coord!));
        //Console.WriteLine("MoveStarshipQuadrant: newQuadrant Coords: " + getCoord(newQuadrant.Coord!));
        //Console.WriteLine(new String('=', 80));
    }

    public List<QuadrantDTO> toQuadrantDTO => Mapper.ToDTO(this.Quadrants);
    public static List<Quadrant> fromQuadrantDTO(List<QuadrantDTO> quadrantDTOs) =>
        Mapper.FromDTO(quadrantDTOs);

}
