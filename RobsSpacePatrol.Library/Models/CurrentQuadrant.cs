using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RobsSpacePatrol.DTO.Library;

[assembly: InternalsVisibleTo("RobsSpacePatrol.Library.Tests")]

namespace RobsSpacePatrol.Library.Models;

public record CurrentQuadrant
{
    public CurrentQuadrant(Coord coord, List<Sector> sectors)
    {
        Coord = coord;
        Sectors = sectors;
    }

    public Coord Coord { get; set; }
    public List<Sector> Sectors { get; set; }

    public Sector? GetSectorByContents(string contents)
    {
        return Sectors.FirstOrDefault(z => z.SectorContents == contents);
    }
    public Sector GetSectorByCoord(Coord coord)
    {
        return Sectors.First(z => z.Coord == coord);
    }
    public SectorDTO GetSectorByCoordDTO(CoordDTO coord)
    {
        return toSectorDTO.First(z => z.Coord == coord);
    }
    public string getFormatted()
    {
        string outString = "";

        outString += "Current Quad Coord: " + Coord.ToString() + "\n";

        for (int _y = -1; _y <= 7; _y++)
        {
            if (_y == -1)
            {
                outString += "###" + "\t";
            }
            else
            {
                outString += _y + "\t";
            }
        }
        outString += "\n";

        for (int _y = 0; _y <= 7; _y++)
        {
            string outRowString = "";
            for (int _x = 0; _x <= 7; _x++)
            {
                var item = Sectors.First(z => z.Coord.X == _x && z.Coord.Y == _y);
                //outRowString += _x + "." + _y + "\t";
                outRowString += item.getSectorFormatted() + "\t";
            }
            outString += _y + "\t" + outRowString + "\n";
        }
        return outString;
    }
    public void moveObjectInsideQuadrant(Coord soureCoord, Coord destCoord)
    {
        var sourceSector = GetSectorByCoord(soureCoord);
        if (sourceSector == null) return;
        var destSector = GetSectorByCoord(destCoord);
        if (destSector == null) return;

        if (destSector.SectorContents != Global.SectorEmpty)
            throw new Exception("Error!  Collision!");

        destSector.SectorContents = sourceSector.SectorContents;
        sourceSector.SectorContents = Global.SectorEmpty;
    }

    //for testing only
    internal void ClearQuadrant(bool includeStarship = false)
    {
        List<Sector> sectorsWithContents;

        if (includeStarship)
            sectorsWithContents = Sectors;
        else
            sectorsWithContents = Sectors.Where(z => z.SectorContents != Global.SectorStarship).ToList();
        sectorsWithContents.ForEach(z => z.SectorContents = Global.SectorEmpty);
        sectorsWithContents.ForEach(z => z.Enemy = null);
    }


    public CurrentQuadrantDTO toDTO => Mapper.ToDTO(this);
    public static CurrentQuadrant fromDTO(CurrentQuadrantDTO currentQuadrantDTO) => Mapper.FromDTO(currentQuadrantDTO);

    public List<SectorDTO> toSectorDTO => Mapper.ToDTO(this.Sectors);
    public static List<Sector> fromSectorDTO(List<SectorDTO> sectorDTOs) =>
        Mapper.FromDTO(sectorDTOs);
}
