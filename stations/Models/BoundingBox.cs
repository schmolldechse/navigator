namespace stations.Models;

public record BoundingBox(double North, double South, double East, double West)
{
    public override string ToString()
    {
        return $"[{North}_{South}_{East}_{West}]";
    }
}