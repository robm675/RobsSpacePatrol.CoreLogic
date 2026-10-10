using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobsSpacePatrol.DTO.Library;

namespace RobsSpacePatrol.DTO.Game;

public record SRS_Response(List<SectorDTO>? sectors, Result.MaintResult? maintResult);
public record LRS_Response(List<QuadrantDTO> quadrants, CoordDTO starshipQuadrantCoord, Result.MaintResult maintResult);
public record DAM_Response(List<DeviceDTO> devices);
public record COM_REC_Response(List<QuadrantDTO> quadrants, CoordDTO currentQuadCoord);
public record COM_NAV_Response(double dir, double dist);
public record COM_STA_Response(List<DeviceDTO> allDevices, double missionTime, CoordDTO quadrantCoord, CoordDTO sectorCoord, int enemiesRemaining, int starBaseRemaining, int energyRemaining, int shieldLevel, int torpedosRemaining, bool docked, double missionTimeDeadLine);
public record COM_STB_Response(CoordDTO starbaseCoord, double dir, double dist);
public record COM_TOR_Response(List<DirectionToEnemyDTO> directionToEnemies);
public record NAV_Response(eNavMessage navMessage, double distanceTravelled, CoordDTO startQuadCoord, CoordDTO finalQuadCoord, CoordDTO finalSectCoord, Result.MaintResult maintResult);
public record LAS_Response(List<ObjectHitReportDTO> objectHitReports, Result.MaintResult maintResult);
public record TOR_Response(ObjectHitReportDTO objectHitReport, bool enemyRemain, int shieldLevel, Result.MaintResult maintResult);
public record SHE_Response(string message, Result.MaintResult maintResult);
