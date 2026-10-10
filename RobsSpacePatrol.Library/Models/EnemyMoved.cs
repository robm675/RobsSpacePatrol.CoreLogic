using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobsSpacePatrol.DTO.Library;

namespace RobsSpacePatrol.Library.Models;
public class EnemyMoved
{
    public EnemyMoved(Coord enemyCoord_Orig, Coord enemyCoord_Dest)
    {
        EnemyCoord_Orig = enemyCoord_Orig;
        EnemyCoord_Dest = enemyCoord_Dest;
    }

    public Coord EnemyCoord_Orig { get; private set; }
    public Coord EnemyCoord_Dest { get; private set; }

    public EnemyMovedDTO toDTO => new EnemyMovedDTO(EnemyCoord_Orig.toDTO, EnemyCoord_Dest.toDTO);
    public static EnemyMoved fromDTO(EnemyMovedDTO enemyMovedDTO) => new EnemyMoved(Coord.fromDTO(enemyMovedDTO.EnemyCoord_Orig), Coord.fromDTO(enemyMovedDTO.EnemyCoord_Dest));
}
