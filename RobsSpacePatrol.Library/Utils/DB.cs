using Force.DeepCloner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobsSpacePatrol.Library.Models;
using System.Text.Json;

namespace RobsSpacePatrol.Library.Utils;
public static class DB
{
	public static string GetJsonFromGalaxy(Galaxy galaxy)
	{
        return JsonSerializer.Serialize(galaxy);
    }
    public static Galaxy GetGalaxyFromJson(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        try
        {
            return JsonSerializer.Deserialize<Galaxy>(json)
                ?? throw new JsonException("The saved game contained no galaxy data.");
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException(
                "The saved game is not valid Rob's Space Patrol data.",
                ex);
        }
    }
}
