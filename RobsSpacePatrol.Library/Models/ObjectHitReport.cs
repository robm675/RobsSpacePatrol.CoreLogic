using RobsSpacePatrol.DTO.Library;

namespace RobsSpacePatrol.Library.Models;

public record ObjectHitReport
{
    public ObjectHitReport(eObjectHit objectHit, bool destroyed, Coord coord, int shieldRemaining, int unitHit, bool missed, bool starSurvied)
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
    public Coord Coord { get; set; }
    public int ShieldRemaining { get; set; }
    public int UnitHit { get; set; }
    public bool Missed { get; set; }
    public bool StarSurvied { get; set; }

    public ObjectHitReportDTO toDTO => Mapper.ToDTO(this);
    public static ObjectHitReport fromDTO(ObjectHitReportDTO objectHitReportDTO) => Mapper.FromDTO(objectHitReportDTO);
}
