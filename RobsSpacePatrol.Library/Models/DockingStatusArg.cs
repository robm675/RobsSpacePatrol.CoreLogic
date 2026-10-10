using RobsSpacePatrol.DTO.Library;

namespace RobsSpacePatrol.Library.Models;

public class DockingStatusArg
{
    public DockingStatusArg(bool previousStatus, bool currentStatus)
    {
        PreviousStatus = previousStatus;
        CurrentStatus = currentStatus;
    }

    public bool PreviousStatus { get; set; }
    public bool CurrentStatus { get; set; }

    public DockingStatusArgDTO toDTO => new DockingStatusArgDTO(PreviousStatus, CurrentStatus);
    public static DockingStatusArg fromDTO(DockingStatusArgDTO dockingStatusArgDTO) =>
        new DockingStatusArg(dockingStatusArgDTO.PreviousStatus, dockingStatusArgDTO.CurrentStatus);

}
