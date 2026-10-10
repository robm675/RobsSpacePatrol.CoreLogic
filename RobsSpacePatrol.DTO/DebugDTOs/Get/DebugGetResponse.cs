using RobsSpacePatrol.DTO.Library;

namespace RobsSpacePatrol.DTO.Debug.Get;

public record DebugGetSectorContentsResponse(string SectorContents) { }
public record DebugGetSectorsByContentResponse(List<SectorDTO> sectors) { }
public record DebugGetEnemyResponse(EnemyDTO? enemy) { }
public record DebugGetQuadrantResponse(QuadrantDTO quadrant) { }
public record DebugGetStarshipResponse(StarshipDTO starship) { }
public record DebugGetGalaxyResponse(double missionTime, double missionTimeDeadline) { }
public record DebugGetStarshipQuadrantResponse(CoordDTO coord) { }
