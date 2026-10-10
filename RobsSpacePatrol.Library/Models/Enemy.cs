using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobsSpacePatrol.DTO.Library;

namespace RobsSpacePatrol.Library.Models;

public record Enemy
{
    public Enemy(int shieldLevel)
    {
        ShieldLevel = shieldLevel;
    }

    public int ShieldLevel { get; set; }

    public EnemyDTO toDTO => new EnemyDTO(ShieldLevel);
    public static Enemy? fromDTO(EnemyDTO? enemyDTO) => enemyDTO == null ? null : new Enemy(enemyDTO.ShieldLevel);
}
