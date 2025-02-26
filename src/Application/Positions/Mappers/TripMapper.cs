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
    public static IEnumerable<TripVm> GroupPositionsIntoTrips(
        this IEnumerable<PositionVm> positions, 
        bool ignitionBased, 
        double stoppedGap,
        double maxDistance, 
        TimeSpan maxTimeGap)
    {
        if (!positions.Any())
        {
            return [];
        }
        var points = positions.CalculatePoints(ignitionBased, stoppedGap);
        var trips = new List<TripVm>();
        var currentTrip = CreateNewTrip();
        TripPointVm? previousPoint = null;

        foreach (var point in points)
        {
            if (previousPoint != null && !IsDuplicate(previousPoint.Value, point))
            {
                var distance = CalculateDistance(previousPoint.Value, point);
                var timeDiff = point.DeviceDateTime - previousPoint.Value.DeviceDateTime;

                if (previousPoint.Value.Movement != point.Movement || distance > maxDistance || timeDiff > maxTimeGap)
                {
                    // End the current trip and start a new one
                    FinalizeTrip(currentTrip, trips, previousPoint.Value.Movement);
                    currentTrip = CreateNewTrip(point);
                }
                else
                {
                    currentTrip.Points.Add(point);
                    currentTrip.TotalDistance += distance;
                }
            }
            if (previousPoint == null) currentTrip.Points.Add(point);
            previousPoint = point;
        }
        // Add the last trip if it has more than one position
        FinalizeTrip(currentTrip, trips, points.Last().Movement);

        return trips;
    }

    /// <summary>
    /// Creates a new trip with an optional initial point.
    /// </summary>
    /// <param name="initialPosition">The initial point of the trip.</param>
    /// <returns>A new trip.</returns>
    private static TripVm CreateNewTrip(TripPointVm? initialPoint = null)
        => new()
        {
            TripId = Guid.NewGuid(),
            Points = initialPoint != null ? [initialPoint.Value] : [],
            TotalDistance = 0,
            Type = (short)TripTypeEnum.MOVING
        };

    /// <summary>
    /// Finalizes the trip by calculating its duration and average speed, and determining its type.
    /// </summary>
    /// <param name="trip">The trip to finalize.</param>
    /// <param name="trips">The list of trips to add the finalized trip to.</param>
    /// <param name="minSpeedThreshold">The minimum speed threshold to consider a trip as moving.</param>
    private static void FinalizeTrip(TripVm trip, List<TripVm> trips, bool movement)
    {
        trip.Duration = trip.Points.Last().DeviceDateTime - trip.Points.First().DeviceDateTime;
        trip.From = trip.Points.First().DeviceDateTime;
        trip.To = trip.Points.Last().DeviceDateTime;
        trip.AverageSpeed = trip.Points.Average(p => p.Speed);
        if (!movement)
        {
            trip.Type = (short)TripTypeEnum.STOP;
            if (trip.Points.Count > 1)
            {
                trip.Points = [trip.Points.First(), trip.Points.Last()];
            }
        }
        trips.Add(trip);
    }

    /// <summary>
    /// Calculates the distance between two points using the Haversine formula.
    /// </summary>
    /// <param name="p1">The first point.</param>
    /// <param name="p2">The second point.</param>
    /// <returns>The distance between the two points in kilometers.</returns>
    private static double CalculateDistance(TripPointVm p1, TripPointVm p2)
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
    /// Checks if two points are duplicates.
    /// </summary>
    /// <param name="p1">The first point.</param>
    /// <param name="p2">The second point.</param>
    /// <returns>True if the points are duplicates, otherwise false.</returns>
    private static bool IsDuplicate(TripPointVm p1, TripPointVm p2)
        => p1.Latitude == p2.Latitude &&
           p1.Longitude == p2.Longitude &&
           p1.DeviceDateTime == p2.DeviceDateTime;

    /// <summary>
    /// Calculates the points of a trip based on ignition status or speed.
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="ignitionBased"></param>
    /// <param name="stoppedGap"></param>
    /// <returns></returns>
    private static List<TripPointVm> CalculatePoints(this IEnumerable<PositionVm> positions, bool ignitionBased, double stoppedGap)
    {
        var points = new List<TripPointVm>();
        var stoppedTime = TimeSpan.Zero;
        PositionVm? previousPosition = null;

        foreach (var position in positions.OrderBy(p => p.DeviceDateTime))
        {
            if (ignitionBased)
            {
                // Add point based on ignition status
                points.Add(position.CastPoint(position.Attributes?.Ignition ?? false));
                continue;
            }

            if (previousPosition != null)
            {
                // Update stopped time if the vehicle is not moving
                stoppedTime = UpdateStoppedTime(position, previousPosition.Value, stoppedTime);
                // Add point based on whether the vehicle should be considered moving
                points.Add(position.CastPoint(ShouldConsiderMoving(position, stoppedTime, stoppedGap)));
            }
            else
            {
                // Add the first point based on speed
                points.Add(position.CastPoint(position.Speed > 0));
            }

            previousPosition = position;
        }

        return points;
    }

    private static TimeSpan UpdateStoppedTime(PositionVm current, PositionVm previous, TimeSpan stoppedTime)
        => current.Speed > 0
            ? TimeSpan.Zero
            : stoppedTime + (current.DeviceDateTime - previous.DeviceDateTime);

    private static bool ShouldConsiderMoving(PositionVm position, TimeSpan stoppedTime, double stoppedGap)
        => position.Speed > 0 || stoppedTime.TotalMinutes < stoppedGap;

    public static TripPointVm CastPoint(this PositionVm position, bool movement)
        => new (
            position.Latitude, 
            position.Longitude, 
            position.DeviceDateTime, 
            position.Speed, 
            position.Course, 
            position.EventId, 
            movement);

}
