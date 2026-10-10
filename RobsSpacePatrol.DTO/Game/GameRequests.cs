using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobsSpacePatrol.DTO.Library;

namespace RobsSpacePatrol.DTO.Game;

public record SRS_Request(Guid gameKey);
public record LRS_Request(Guid gameKey);
public record DAM_Request(Guid gameKey);
public record COM_REC_Request(Guid gameKey);
public record COM_NAV_Request(Guid gameKey, CoordDTO quadrantCoord, CoordDTO sectorCoord);
public record COM_STA_Request(Guid gameKey);
public record COM_STB_Request(Guid gameKey);
public record COM_TOR_Request(Guid gameKey);
public record NAV_Request(Guid gameKey, double dir, double dist);
public record LAS_Request(Guid gameKey, int energy);
public record TOR_Request(Guid gameKey, double dir);
public record SHE_Request(Guid gameKey, int energy);

public record ZAP_Game_Request(Guid gameKey);