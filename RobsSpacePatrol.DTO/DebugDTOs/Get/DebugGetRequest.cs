using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobsSpacePatrol.DTO.Library;

namespace RobsSpacePatrol.DTO.Debug.Get;

public record DebugGetSectorContentsRequest(Guid GameKey, CoordDTO Coord)  { }
public record DebugGetSectorsByContentRequest(Guid GameKey, string contents) { }
public record DebugGetEnemyRequest(Guid GameKey, CoordDTO Coord)  { }
public record DebugGetQuadrantRequest(Guid GameKey, CoordDTO Coord) { }
public record DebugGetStarshipRequest(Guid GameKey) { }
public record DebugGetGalaxyRequest(Guid GameKey) { }
public record DebugGetStarshipQuadrantRequest(Guid GameKey) { }
