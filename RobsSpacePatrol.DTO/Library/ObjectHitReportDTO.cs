namespace RobsSpacePatrol.DTO.Library;

public record ObjectHitReportDTO
{
    public ObjectHitReportDTO(eObjectHit objectHit, bool destroyed, CoordDTO coord, int shieldRemaining, int unitHit, bool missed, bool starSurvied)
    {
        ObjectHit = objectHit;
        Destroyed = destroyed;
        Coord = coord;
        ShieldRemaining = shieldRemaining;
        UnitHit = unitHit;
        Missed = missed;
        StarSurvied = starSurvied;
    }

    public eObjectHit ObjectHit { get; set; }
    public bool Destroyed { get; set; }
    public CoordDTO Coord { get; set; }
    public int ShieldRemaining { get; set; }
    public int UnitHit { get; set; }
    public bool Missed { get; set; }
    public bool StarSurvied { get; set; }
}
