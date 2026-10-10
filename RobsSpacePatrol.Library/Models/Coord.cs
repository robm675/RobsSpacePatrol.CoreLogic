using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RobsSpacePatrol.DTO.Library;

namespace RobsSpacePatrol.Library.Models;


public record Coord
{
    [JsonConstructor]
    public Coord(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public override string ToString()
    {
        return $"{X}.{Y}";
    }
    public bool isSame(CoordDTO coord)
    {
        return coord.X == X && coord.Y == Y;
    }

    public CoordDTO toDTO => new CoordDTO(X, Y);
    public static Coord fromDTO(CoordDTO coordDTO) => new Coord(coordDTO.X, coordDTO.Y);
}
