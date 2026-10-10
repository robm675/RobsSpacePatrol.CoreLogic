using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RobsSpacePatrol.Library.Models;
public record QS
{
    public QS(int s, int q)
    {
        S = s;
        Q = q;
    }

    public int Q { get; }
    public int S { get; }

    public override string ToString()
    {
        return $"S = {S} Q = {Q}";
    }

    public static bool operator >(QS? left, QS? right)
    {
        if (left is null || right is null)
            return false;

        return left.Q > right.Q ||
               left.Q == right.Q && left.S > right.S;
    }
    public static bool operator <(QS? left, QS? right)
    {
        if (left is null || right is null)
            return false;

        return left.Q < right.Q ||
               left.Q == right.Q && left.S < right.S;
    }
    public static bool operator <=(QS? left, QS? right)
    {
        if (left is null || right is null) return false;

        if (left.Q < right.Q) return true;
        if (left.Q == right.Q && left.S < right.S) return true;
        if (left == right) return true;
        return false;
    }
    public static bool operator >=(QS? left, QS? right)
    {
        if (left is null || right is null) return false;

        if (left.Q > right.Q) return true;
        if (left.Q == right.Q && left.S >= right.S) return true;
        if (left == right) return true;
        return false;
    }
}
