using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobsSpacePatrol.Library.Models;

namespace RobsSpacePatrol.Library.Factories.Interfaces;
public interface IQuadrantFactory
{
    Quadrant createQuadrant(Coord coord, IRandomFactory randomFactory);
}
