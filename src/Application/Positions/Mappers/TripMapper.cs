using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.Positions.Mappers;

public static class TripMapper
{
    public static IEnumerable<TripVm> GroupPositionsIntoTrips(this IEnumerable<PositionVm> positions, double maxDistance, TimeSpan maxTimeGap, double minSpeedThreshold = 0.1)
    {
        var trips = new List<TripVm>();
        var currentTrip = new TripVm
        {
            TripId = Guid.NewGuid(),
            Points = [],
            TotalDistance = 0,
        };

        PositionVm? lastPosition = null;

        foreach (var position in positions.OrderBy(p => p.DeviceDateTime))
        {
            if (lastPosition != null && !IsDuplicate(lastPosition.Value, position))
            {
                var distance = CalculateDistance(lastPosition.Value, position);
                var timeGap = position.DeviceDateTime - lastPosition.Value.DeviceDateTime;

                if (distance > maxDistance || timeGap > maxTimeGap)
                {
                    // End the current trip and start a new one
                    if (currentTrip.Points.Count > 1 && !IsStop(currentTrip.Points, minSpeedThreshold))
                    {
                        currentTrip.Duration = currentTrip.Points.Last().DeviceDateTime - currentTrip.Points.First().DeviceDateTime;
                        currentTrip.AverageSpeed = currentTrip.Points.Average(p => p.Speed);
                        trips.Add(currentTrip);
                    }

                    currentTrip = new TripVm
                    {
                        TripId = Guid.NewGuid(),
                        Points = [position],
                        TotalDistance = 0
                    };
                }
                else
                {
                    currentTrip.Points.Add(position);
                    currentTrip.TotalDistance += distance;
                }
            }
            if (lastPosition == null) currentTrip.Points.Add(position);
            lastPosition = position;
        }

        // Add the last trip if it has more than one position or is not a stop
        if (currentTrip.Points.Count > 1 || !IsStop(currentTrip.Points, minSpeedThreshold))
        {
            currentTrip.Duration = currentTrip.Points.Last().DeviceDateTime - currentTrip.Points.First().DeviceDateTime;
            currentTrip.AverageSpeed = currentTrip.Points.Average(p => p.Speed);
            trips.Add(currentTrip);
        }

        // Ensure at least one trip is returned with one position if all positions are in the same place
        if (trips.Count == 0 && positions.Any())
        {
            var singlePositionTrip = new TripVm
            {
                TripId = Guid.NewGuid(),
                Points = [positions.First()],
                TotalDistance = 0,
                Duration = TimeSpan.Zero,
                AverageSpeed = positions.First().Speed
            };
            trips.Add(singlePositionTrip);
        }

        return trips;
    }

    private static double CalculateDistance(PositionVm p1, PositionVm p2)
    {
        // Haversine formula to calculate the distance between two points on the Earth's surface
        const double r = 6371e3; // Earth's radius in meters
        var lat1 = p1.Latitude * Math.PI / 180;
        var lat2 = p2.Latitude * Math.PI / 180;
        var deltaLat = (p2.Latitude - p1.Latitude) * Math.PI / 180;
        var deltaLon = (p2.Longitude - p1.Longitude) * Math.PI / 180;

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return r * c / 1000; // Convert to kilometers
    }

    private static bool IsDuplicate(PositionVm p1, PositionVm p2)
    {
        return p1.Latitude == p2.Latitude &&
               p1.Longitude == p2.Longitude &&
               p1.DeviceDateTime == p2.DeviceDateTime;
    }

    private static bool IsStop(List<PositionVm> points, double minSpeedThreshold)
    {
        return points.All(p => p.Speed <= minSpeedThreshold);
    }
}
