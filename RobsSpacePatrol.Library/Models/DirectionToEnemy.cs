using RobsSpacePatrol.DTO.Library;

namespace RobsSpacePatrol.Library.Models;

public record DirectionToEnemy(double direction, double distance, Coord coord)
{
    public DirectionToEnemyDTO toDTO => new DirectionToEnemyDTO(direction, distance, coord.toDTO);
    public static DirectionToEnemy fromDTO(DirectionToEnemyDTO directionToEnemyDTO) =>
        new DirectionToEnemy(directionToEnemyDTO.Direction, directionToEnemyDTO.Distance, Coord.fromDTO(directionToEnemyDTO.Coord));
}
