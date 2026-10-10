using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobsSpacePatrol.DTO.Library;
public record SectorDTO(CoordDTO Coord, string SectorContents, EnemyDTO? Enemy) { }
