using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RobsSpacePatrol.DTO.Library;

namespace RobsSpacePatrol.Library.Models;

public record Sector
{
    public Sector(Coord coord, string sectorContents)
    {
        Coord = coord;
        SectorContents = sectorContents;
    }

    [JsonConstructor]
    public Sector(Coord coord, string sectorContents, Enemy? enemy) : this(coord, sectorContents)
    {
        Enemy = enemy;
    }

    public Coord Coord { get; set; }
    public string SectorContents { get; set; }
    public Enemy? Enemy { get; set; } = null;

    [JsonIgnore]
    public bool IsEmpty => SectorContents == Global.SectorEmpty;
    [JsonIgnore]
    public bool HasStarship => SectorContents == Global.SectorStarship;
    [JsonIgnore]
    public bool HasEnemy => SectorContents == Global.SectorEnemy;
    [JsonIgnore]
    public bool HasStar => SectorContents == Global.SectorStar;
    [JsonIgnore]
    public bool HasStarbase => SectorContents == Global.SectorStarBase;

    public void SetEmpty() { SectorContents = Global.SectorEmpty;  Enemy = null; }
    public void SetStarship() { SectorContents = Global.SectorStarship; Enemy = null; }
    public void SetStar() { SectorContents = Global.SectorStar; Enemy = null; }
    public void SetStarbase() { SectorContents = Global.SectorStarBase; Enemy = null; }
    public void SetEnemy(int shieldLevel) { SectorContents = Global.SectorEnemy; Enemy = new(shieldLevel); }

    public string getSectorFormatted()
    {
        switch (SectorContents)
        {
            case Global.SectorEnemy:
                return "XXX";

            case Global.SectorStarship:
                return "SSS";

            case Global.SectorEmpty:
                return "   ";

            case Global.SectorStar:
                return "***";

            case Global.SectorStarBase:
                return "BBB";

            default:
                return "???";
        }
    }

    public SectorDTO toDTO => new SectorDTO(Coord.toDTO, SectorContents, Enemy?.toDTO);
    public static Sector fromDTO(SectorDTO sectorDTO) => new Sector(Coord.fromDTO(sectorDTO.Coord), sectorDTO.SectorContents, Enemy.fromDTO(sectorDTO.Enemy));
}
