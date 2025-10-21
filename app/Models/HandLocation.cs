using System.Text.Json;
using System.Text.Json.Serialization;

namespace LMStreamer;

public class Vector(double x, double y, double z)
{
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
    public double Z { get; set; } = z;
    [JsonIgnore]
    public bool IsZero => X == 0 && Y == 0 && Z == 0;
    public static Vector Zero => new(0, 0, 0);
    public static Vector From(ref readonly Leap.Vector v) => new(Math.Round(v.x, 2), Math.Round(v.y, 2), Math.Round(v.z, 2));
    public static Vector operator /(Vector obj, double div) => new(obj.X / div, obj.Y / div, obj.Z / div);
    public static Vector operator *(Vector obj, double mul) => new(obj.X * mul, obj.Y * mul, obj.Z * mul);
}

public class HandLocation(Vector palm, Vector thumb, Vector index, Vector middle)
{
    public Vector Palm { get; set; } = palm;
    public Vector Thumb { get; set; } = thumb;
    public Vector Index { get; set; } = index;
    public Vector Middle { get; set; } = middle;
    [JsonIgnore]
    public bool IsEmpty => Palm.IsZero && Thumb.IsZero && Index.IsZero && Middle.IsZero;
    public HandLocation() : this(Vector.Zero, Vector.Zero, Vector.Zero, Vector.Zero) { }
    public string AsJson() => JsonSerializer.Serialize(this);
    public static HandLocation Empty => new();
}
