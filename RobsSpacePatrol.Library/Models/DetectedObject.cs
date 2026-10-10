using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobsSpacePatrol.Library.Models;
public record DetectedObject
{
    public string ObjectHit;
    public double? targetDIST;
    public Coord? FinalCoord;
    public List<Coord>? TrackingCoords;

    public DetectedObject(string objectHit)
    {
        ObjectHit = objectHit;
        targetDIST = null;
        FinalCoord = null;
        TrackingCoords = null;
    }

    public DetectedObject(string objectHit, double? targetDIST, Coord? finalCoord, List<Coord>? trackingCoords)
    {
        ObjectHit = objectHit;
        this.targetDIST = targetDIST;
        FinalCoord = finalCoord;
        TrackingCoords = trackingCoords;
    }
}
