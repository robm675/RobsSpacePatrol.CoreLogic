namespace RobsSpacePatrol.Library.Models;

public record StarbaseRepairs
{
    public double MissionTimeUnitsToRepair { get; private set; }
    public Action<double> StarbaseMakeRepairs { get; private set; }

    public StarbaseRepairs(double missionTimeUnitsToRepair, Action<double> starbaseMakeRepairs)
    {
        MissionTimeUnitsToRepair = missionTimeUnitsToRepair;
        StarbaseMakeRepairs = starbaseMakeRepairs;
    }
}
