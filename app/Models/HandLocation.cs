//using Leap;
using System.Text.Json;

namespace LMStreaming;
/*
public class HandLocation(ref readonly Vector palm, ref readonly Vector thumb, ref readonly Vector index, ref readonly Vector middle)
{
    public Vector Palm { get; set; } = palm;
    public Vector Thumb { get; set; } = thumb;
    public Vector Index { get; set; } = index;
    public Vector Middle { get; set; } = middle;

    public bool IsEmpty { get; private set; } = false;

    public HandLocation() : this(in Vector.Zero, in Vector.Zero, in Vector.Zero, in Vector.Zero)
    {
        IsEmpty = true;
    }

    public void CopyTo(HandLocation lhs)
    {
        lhs.Palm = Palm;
        lhs.Thumb = Thumb;
        lhs.Index = Index;
        lhs.Middle = Middle;
        lhs.IsEmpty = IsEmpty;
    }
}*/

public class Vector(double x, double y, double z)
{
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
    public double Z { get; set; } = z;
    public double Magnitude => Math.Sqrt(X * X + Y * Y + Z * Z);
    public bool IsZero => X == 0 && Y == 0 && Z == 0;
    public static Vector Zero => new(0, 0, 0);
    public static Vector From(ref readonly Leap.Vector v) => new(v.x, v.y, v.z);
    public static Vector From(string s) => From(s.Split([',', ' ', ';']));
    public static Vector From(string[] s) => s.Length >= 3 &&
               double.TryParse(s[0], out double x) &&
               double.TryParse(s[1], out double y) &&
               double.TryParse(s[2], out double z) ?
               new Vector(x, y, z) : Zero;
    public static Vector operator /(Vector obj, double div) => new(obj.X / div, obj.Y / div, obj.Z / div);
    public static Vector operator *(Vector obj, double mul) => new(obj.X * mul, obj.Y * mul, obj.Z * mul);
}

public class HandLocation(Vector palm, Vector thumb, Vector index, Vector middle)
{
    public Vector Palm { get; set; } = palm;
    public Vector Thumb { get; set; } = thumb;
    public Vector Index { get; set; } = index;
    public Vector Middle { get; set; } = middle;
    public HandLocation() : this(Vector.Zero, Vector.Zero, Vector.Zero, Vector.Zero) { }
    public void CopyTo(HandLocation rhs)
    {
        rhs.Palm = Palm;
        rhs.Thumb = Thumb;
        rhs.Index = Index;
        rhs.Middle = Middle;
    }
    public string AsJson() => JsonSerializer.Serialize(this);

    public static HandLocation Empty => new();
    public static HandLocation FromRemoteString(string s)
    {
        if (string.IsNullOrEmpty(s))
            return Empty;

        var result = new HandLocation();
        var p = s.Split(" ");

        try
        {
            if (p.Length >= 3)
                result.Palm = Vector.From(p[0..3]);
            if (p.Length >= 6)
                result.Thumb = Vector.From(p[3..6]);
            if (p.Length >= 9)
                result.Index = Vector.From(p[6..9]);
            if (p.Length >= 12)
                result.Middle = Vector.From(p[9..12]);
        }
        catch { }

        return result;
    }
    public static HandLocation FromRemoteJson(string json)
    {
        if (string.IsNullOrEmpty(json))
            return Empty;

        HandLocation result = Empty;

        try
        {
            result = JsonSerializer.Deserialize<HandLocation>(json) ?? Empty;
        }
        catch { }

        return result;
    }
}
