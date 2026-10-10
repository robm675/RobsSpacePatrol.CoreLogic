namespace RobsSpacePatrol.DTO.Library;

public record DockingStatusArgDTO
{
    public bool PreviousStatus { get; set; }
    public bool CurrentStatus { get; set; }


    public DockingStatusArgDTO(bool previousStatus, bool currentStatus)
    {
        PreviousStatus = previousStatus;
        CurrentStatus = currentStatus;
    }
}
