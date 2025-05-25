namespace stations.Models;

public record BoundingBox(double North, double West, double South, double East)
{
    public override string ToString()
    {
        return $"[{North}_{West}_{South}_{East}]";
    }
}