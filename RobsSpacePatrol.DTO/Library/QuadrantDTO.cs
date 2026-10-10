using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobsSpacePatrol.DTO.Library;
public record QuadrantDTO
{
    public QuadrantDTO(CoordDTO coord, int numberOfEnemies, int numberOfStars, bool hasBeenExplored, bool hasStarBase)
    {
        Coord = coord;
        NumberOfEnemies = numberOfEnemies;
        NumberOfStars = numberOfStars;
        HasBeenExplored = hasBeenExplored;
        HasStarBase = hasStarBase;
    }

    public CoordDTO Coord { get; set; }
    public int NumberOfEnemies { get; set; }
    public int NumberOfStars { get; set; }
    public bool HasBeenExplored { get; set; }
    public bool HasStarBase { get; set; }

}
