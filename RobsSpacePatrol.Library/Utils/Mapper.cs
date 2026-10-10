using Riok.Mapperly.Abstractions;
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library.Models;

namespace RobsSpacePatrol.Library.Utils;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class Mapper
{
    internal static partial ObjectHitReportDTO ToDTO(ObjectHitReport source);
    internal static partial ObjectHitReport FromDTO(ObjectHitReportDTO source);

    internal static partial CoordDTO ToDTO(Coord source);
    internal static partial Coord FromDTO(CoordDTO source);

    internal static partial EnemyFiredDTO ToDTO(EnemyFired source);
    internal static partial EnemyFired FromDTO(EnemyFiredDTO source);

    [MapProperty(nameof(CurrentQuadrant.Coord), nameof(CurrentQuadrantDTO.CoordDTO))]
    internal static partial CurrentQuadrantDTO ToDTO(CurrentQuadrant source);

    [MapProperty(nameof(CurrentQuadrantDTO.CoordDTO), nameof(CurrentQuadrant.Coord))]
    internal static partial CurrentQuadrant FromDTO(CurrentQuadrantDTO source);

    internal static partial QuadrantDTO ToDTO(Quadrant source);
    internal static partial Quadrant FromDTO(QuadrantDTO source);

    internal static partial SectorDTO ToDTO(Sector source);
    internal static partial Sector FromDTO(SectorDTO source);

    internal static partial StarshipDTO ToDTO(Starship source);
    internal static partial Starship FromDTO(StarshipDTO source);

    internal static partial DeviceDTO ToDTO(Device source);
    internal static partial Device FromDTO(DeviceDTO source);

    internal static partial EnemyDTO ToDTO(Enemy source);
    internal static partial Enemy? FromDTO(EnemyDTO? source);

    internal static partial List<SectorDTO> ToDTO(List<Sector> source);
    internal static partial List<Sector> FromDTO(List<SectorDTO> source);
    internal static partial List<DeviceDTO> ToDTO(List<Device> source);
    internal static partial List<Device> FromDTO(List<DeviceDTO> source);
    internal static partial List<QuadrantDTO> ToDTO(List<Quadrant> source);
    internal static partial List<Quadrant> FromDTO(List<QuadrantDTO> source);
}
