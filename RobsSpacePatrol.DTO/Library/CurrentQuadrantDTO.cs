
namespace RobsSpacePatrol.DTO.Library;

public class CurrentQuadrantDTO
{
    public CurrentQuadrantDTO(CoordDTO coordDTO, List<SectorDTO> sectors)
    {
        CoordDTO = coordDTO;
        Sectors = sectors;
    }
    public CurrentQuadrantDTO()
    {

    }

    public CoordDTO? CoordDTO { get; set; }
    public List<SectorDTO>? Sectors { get; set; }
}
