using TrackHubRouter.Domain.Enumerators;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.Positions.Mappers;

public static class TripMapper
{
    /// <summary>
    /// Groups positions into trips based on maximum distance and time gap.
    /// </summary>
    /// <param name="positions">The positions to group.</param>
    /// <param name="maxDistance">The maximum distance between points in a trip.</param>
    /// <param name="maxTimeGap">The maximum time gap between points in a trip.</param>
    /// <param name="minSpeedThreshold">The minimum speed threshold to consider a trip as moving.</param>
    /// <returns>A collection of trips.</returns>
    public static IEnumerable<TripVm> GroupPositionsIntoTrips(this IEnumerable<PositionVm> positions, double maxDistance, TimeSpan maxTimeGap, double minSpeedThreshold = 0.1)
    {
        var trips = new List<TripVm>();
        var currentTrip = CreateNewTrip();
        PositionVm? lastPosition = null;

        foreach (var position in positions.OrderBy(p => p.DeviceDateTime))
        {
            if (lastPosition != null && !IsDuplicate(lastPosition.Value, position))
            {
                var distance = CalculateDistance(lastPosition.Value, position);
                var timeGap = position.DeviceDateTime - lastPosition.Value.DeviceDateTime;

                if (distance > maxDistance || timeGap > maxTimeGap)
                {
                    FinalizeTrip(currentTrip, trips, minSpeedThreshold);
                    currentTrip = CreateNewTrip(position);
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

        FinalizeTrip(currentTrip, trips, minSpeedThreshold);

        if (trips.Count == 0 && positions.Any())
        {
            var singlePositionTrip = new TripVm
            {
                TripId = Guid.NewGuid(),
                Points = [positions.First(), positions.Last()],
                TotalDistance = 0,
                Duration = positions.Last().DeviceDateTime - positions.First().DeviceDateTime,
                AverageSpeed = positions.First().Speed,
                Type = 1
            };
            trips.Add(singlePositionTrip);
        }

        return trips;
    }

    /// <summary>
    /// Creates a new trip with an optional initial position.
    /// </summary>
    /// <param name="initialPosition">The initial position of the trip.</param>
    /// <returns>A new trip.</returns>
    private static TripVm CreateNewTrip(PositionVm? initialPosition = null)
    {
        return new TripVm
        {
            TripId = Guid.NewGuid(),
            Points = initialPosition != null ? [initialPosition.Value] : [],
            TotalDistance = 0,
            Type = (short)TripTypeEnum.MOVING
        };
    }

    /// <summary>
    /// Finalizes the trip by calculating its duration and average speed, and determining its type.
    /// </summary>
    /// <param name="trip">The trip to finalize.</param>
    /// <param name="trips">The list of trips to add the finalized trip to.</param>
    /// <param name="minSpeedThreshold">The minimum speed threshold to consider a trip as moving.</param>
    private static void FinalizeTrip(TripVm trip, List<TripVm> trips, double minSpeedThreshold)
    {
        if (trip.Points.Count > 1)
        {
            trip.Duration = trip.Points.Last().DeviceDateTime - trip.Points.First().DeviceDateTime;
            trip.AverageSpeed = trip.Points.Average(p => p.Speed);
            if (IsStop(trip.Points, minSpeedThreshold))
            {
                trip.Type = (short)TripTypeEnum.STOP;
                trip.Points = [trip.Points.First(), trip.Points.Last()];
            }
            trips.Add(trip);
        }
    }

    /// <summary>
    /// Calculates the distance between two positions using the Haversine formula.
    /// </summary>
    /// <param name="p1">The first position.</param>
    /// <param name="p2">The second position.</param>
    /// <returns>The distance between the two positions in kilometers.</returns>
    private static double CalculateDistance(PositionVm p1, PositionVm p2)
    {
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

    /// <summary>
    /// Checks if two positions are duplicates.
    /// </summary>
    /// <param name="p1">The first position.</param>
    /// <param name="p2">The second position.</param>
    /// <returns>True if the positions are duplicates, otherwise false.</returns>
    private static bool IsDuplicate(PositionVm p1, PositionVm p2)
    {
        return p1.Latitude == p2.Latitude &&
               p1.Longitude == p2.Longitude &&
               p1.DeviceDateTime == p2.DeviceDateTime;
    }

    /// <summary>
    /// Determines if a trip is a stop based on the speed of its points.
    /// </summary>
    /// <param name="points">The points of the trip.</param>
    /// <param name="minSpeedThreshold">The minimum speed threshold to consider a trip as moving.</param>
    /// <returns>True if the trip is a stop, otherwise false.</returns>
    private static bool IsStop(IEnumerable<PositionVm> points, double minSpeedThreshold)
    {
        return points.All(p => p.Speed <= minSpeedThreshold);
    }
}
