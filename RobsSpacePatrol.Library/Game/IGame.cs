using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library.Factories.Interfaces;
using RobsSpacePatrol.Library.Models;

namespace RobsSpacePatrol.Library;

public interface IGame
{
    ICurrentQuadrantFactory CurrentQuadrantFactory { get; }
    IGalaxy Galaxy { get; }
    IRandomFactory RandomFactory { get; }


    Result COM_NAV(CoordDTO destQuadrant, CoordDTO destSector);

    Result COM_REC();

    Result COM_STA();

    Result COM_STB();

    Result COM_TOR();

    Result DAM();

    Result LRS();

    Result NAV(double DIR, double DIST);

    Result LAS(int energy);

    Result RunRoutineMaintenance(double changeMissionTime);

    Result SHE(int newShieldLevel);

    Result SRS();

    Result TOR(double DIR);

    void ExecuteStarbaseRepairs(double MissionTimeUnits);
}