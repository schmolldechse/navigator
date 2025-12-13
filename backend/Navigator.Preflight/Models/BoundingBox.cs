namespace Navigator.Preflight.Models;

public record BoundingBox(double North, double West, double South, double East)
{
    public override string ToString() => $"[{North}_{West}_{South}_{East}]";
}
