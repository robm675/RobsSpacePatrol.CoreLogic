using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobsSpacePatrol.DTO.Game;
public record GameStartResponse(Guid _gameID)
{
    public Guid GameID => _gameID;
}
public record DeleteGameResponse(bool deletedOK);