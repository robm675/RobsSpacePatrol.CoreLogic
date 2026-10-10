using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobsSpacePatrol.DTO.Library;

namespace RobsSpacePatrol.Library.Models;
public record Quadrant
{
    public Quadrant(Coord coord, int numberOfEnemies, int numberOfStars, bool hasBeenExplored, bool hasStarBase)
    {
        Coord = coord;
        NumberOfEnemies = numberOfEnemies;
        NumberOfStars = numberOfStars;
        HasBeenExplored = hasBeenExplored;
        HasStarBase = hasStarBase;
    }

    public Coord Coord { get; set; }
    public int NumberOfEnemies { get; set; }
    public int NumberOfStars { get; set; }
    public bool HasBeenExplored { get; set; }
    public bool HasStarBase { get; set; }

    public string getQuadrantForGalaxyFormatted(bool MarkStarship = false)
    {
        string outstr = "";

        if (!HasBeenExplored) { return "| *** "; }

        string starshipMarkerStart = " ";
        string starshipMarkerEnd = " ";

        if (MarkStarship)
        {
            starshipMarkerStart = ">";
            starshipMarkerEnd = "<";
        }

        outstr += "|" + starshipMarkerStart;
        outstr += NumberOfEnemies;

        outstr += HasStarBase ? "1" : "0";

        outstr += NumberOfStars;
        outstr += starshipMarkerEnd;

        return outstr;
    }

    public QuadrantDTO toDTO => Mapper.ToDTO(this);
    public static Quadrant fromDTO(QuadrantDTO quadrantDTO) => Mapper.FromDTO(quadrantDTO);
}
