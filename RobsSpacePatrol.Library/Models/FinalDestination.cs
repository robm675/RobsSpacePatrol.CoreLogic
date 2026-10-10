using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobsSpacePatrol.Library.Models;
public record FinalDestination
{
    public Coord FinalQuadrantCoord { get; }
    public Coord FinalSectorCoord { get; }
    public bool OutsideGalaxy { get; }
    public bool ObjectHit { get; }

    public FinalDestination(Coord finalQuadrantCoord, Coord finalSectorCoord, bool outsideGalaxy, bool objectHit)
    {
        FinalQuadrantCoord = finalQuadrantCoord;
        FinalSectorCoord = finalSectorCoord;
        OutsideGalaxy = outsideGalaxy;
        ObjectHit = objectHit;
    }
}
