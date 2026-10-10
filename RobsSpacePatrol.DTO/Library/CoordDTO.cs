using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobsSpacePatrol.DTO.Library;

public record CoordDTO(int X, int Y)
{
    public override string ToString()
    {
        return $"{X}.{Y}";
    }
    public bool isSame(CoordDTO coord)
    {
        return coord.X == X && coord.Y == Y;
    }
}
