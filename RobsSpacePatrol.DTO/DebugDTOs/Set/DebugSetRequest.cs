using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobsSpacePatrol.DTO.Library;

namespace RobsSpacePatrol.DTO.Debug.Set;

public record DebugSetSectorContentsRequest(Guid GameKey, CoordDTO Coord, string contents) { }
public record DebugSetEnemyRequest(Guid GameKey, CoordDTO Coord, EnemyDTO enemy) { }
public record DebugSetQuadrantRequest(Guid GameKey, CoordDTO Coord, QuadrantDTO quadrant) { }
public record DebugSetStarshipRequest(Guid GameKey, StarshipDTO starship) { }
public record DebugSetGalaxyRequest(Guid GameKey, double missionTime = -1, double missionTimeDeadline = -1) { }
public record DebugSetClearCurrentQuadrantRequest(Guid GameKey, bool all);
public record DebugSetEnemyFireMoveRequest(Guid GameKey, bool stopEnemyFire = false, bool skipEnemyMove = false, bool forceEnemyMove = false);
public record DebugSetStarshipQuadrantRequest(Guid GameKey, CoordDTO coord);
