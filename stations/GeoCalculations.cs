using stations.Models;

namespace stations;

public class GeoCalculations
{
    private const double EarthRadius = 6371000;

    public static (double HalfX, double HalfY) GetHalfBoundingBox(BoundingBox boundingBox) => (
        (boundingBox.East - boundingBox.West) / 2, (boundingBox.North - boundingBox.South) / 2);

    public static GeoPosition GetBoundingBoxCenter(BoundingBox boundingBox)
    {
        var (halfX, halfY) = GetHalfBoundingBox(boundingBox);

        double centerLatitude = halfY + boundingBox.South;
        double centerLongitude;

        if (boundingBox.West <= boundingBox.East)
        {
            centerLongitude = boundingBox.West + halfX;
        }
        else
        {
            centerLongitude = (boundingBox.East + boundingBox.West + 360) / 2;
            if (centerLongitude > 180) centerLongitude -= 360;
        }

        return new(Latitude: centerLatitude, Longitude: centerLongitude);
    }

    public static BoundingBox GetBoundingBoxQuarter(BoundingBox boundingBox, int quarter)
    {
        var (halfX, halfY) = GetHalfBoundingBox(boundingBox);

        var quadrantRow = quarter / 2;
        var quadrantCol = quarter % 2;

        return new(
            North: boundingBox.North - quadrantRow * halfY,
            West: boundingBox.West + quadrantCol * halfX,
            South: boundingBox.South + (1 - quadrantRow) * halfY,
            East: boundingBox.East - (1 - quadrantCol) * halfX
        );
    }

    public static double CalculateDistanceMeters(GeoPosition position1, GeoPosition position2)
    {
        double lat1Rad = position1.Latitude * Math.PI / 180;
        double lat2Rad = position2.Latitude * Math.PI / 180;

        var deltaLonRad = (position2.Longitude - position1.Longitude) * Math.PI / 180;

        var a = Math.Sin(lat1Rad) * Math.Sin(lat2Rad)
                + Math.Cos(lat1Rad) * Math.Cos(lat2Rad) * Math.Cos(deltaLonRad);
        return Math.Acos(Math.Clamp(a, -1, 1)) * EarthRadius;
    }

    public static int CalculateRadius(BoundingBox boundingBox, GeoPosition center, double paddingRatio = 1.1)
    {
        var distance = CalculateDistanceMeters(center, new(Latitude: boundingBox.South, Longitude: boundingBox.East));
        return (int)Math.Round(distance * paddingRatio);
    }
}