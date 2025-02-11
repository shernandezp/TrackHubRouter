﻿using TrackHubRouter.Domain.Models;
using TrackHubRouter.Application.Positions.Mappers;
using FluentAssertions;

namespace Application.UnitTests;
[TestFixture]
public class TripMapperTests
{
    private readonly double _maxDistance = 0.5;
    private readonly TimeSpan _maxTimeGap = TimeSpan.FromMinutes(15);

    [Test]
    public void GroupPositionsIntoTrips_NoPositions_ReturnsEmptyList()
    {
        // Arrange
        var positions = new List<PositionVm>();

        // Act
        var result = positions.GroupPositionsIntoTrips(_maxDistance, _maxTimeGap);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void GroupPositionsIntoTrips_SinglePosition_ReturnsSingleTrip()
    {
        // Arrange
        var positions = new List<PositionVm>
            {
                new() { DeviceDateTime = DateTime.UtcNow, Latitude = 0, Longitude = 0, Speed = 0 }
            };

        // Act
        var result = positions.GroupPositionsIntoTrips(_maxDistance, _maxTimeGap);

        // Assert
        result.Count().Should().Be(1);
        result.First().Points.Count.Should().Be(1);
    }

    [Test]
    public void GroupPositionsIntoTrips_MultiplePositionsWithinThresholds_ReturnsSingleTrip()
    {
        // Arrange
        var positions = new List<PositionVm>
            {
                new() { DeviceDateTime = DateTime.UtcNow, Latitude = 0, Longitude = 0, Speed = 0 },
                new() { DeviceDateTime = DateTime.UtcNow.AddMinutes(1), Latitude = 0.001, Longitude = 0.001, Speed = 1 }
            };

        // Act
        var result = positions.GroupPositionsIntoTrips(_maxDistance, _maxTimeGap);

        // Assert
        result.Count().Should().Be(1);
        result.First().Points.Count.Should().Be(2);
    }

    [Test]
    public void GroupPositionsIntoTrips_MultiplePositionsExceedingThresholds_ReturnsMultipleTrips()
    {
        // Arrange
        var positions = new List<PositionVm>
            {
                new() { DeviceDateTime = DateTime.UtcNow.AddMinutes(1), Latitude = 0.001, Longitude = 0.001, Speed = 10 },
                new() { DeviceDateTime = DateTime.UtcNow.AddMinutes(1), Latitude = 0.002, Longitude = 0.002, Speed = 9 },
                new() { DeviceDateTime = DateTime.UtcNow.AddMinutes(1), Latitude = 0.011, Longitude = 0.011, Speed = 10 },
                new() { DeviceDateTime = DateTime.UtcNow.AddMinutes(1), Latitude = 0.012, Longitude = 0.012, Speed = 9 },
                new() { DeviceDateTime = DateTime.UtcNow.AddMinutes(60), Latitude = 0.013, Longitude = 0.013, Speed = 20 },
                new() { DeviceDateTime = DateTime.UtcNow.AddMinutes(1), Latitude = 0.014, Longitude = 0.014, Speed = 20 }
            };

        // Act
        var result = positions.GroupPositionsIntoTrips(_maxDistance, _maxTimeGap);

        // Assert
        result.Count().Should().Be(3);
    }

    [Test]
    public void GroupPositionsIntoTrips_StopsAndStarts_ReturnsCorrectTrips()
    {
        // Arrange
        var positions = new List<PositionVm>
            {
                new() { DeviceDateTime = DateTime.UtcNow, Latitude = 0, Longitude = 0, Speed = 0 },
                new() { DeviceDateTime = DateTime.UtcNow.AddMinutes(1), Latitude = 0.001, Longitude = 0.001, Speed = 1 },
                new() { DeviceDateTime = DateTime.UtcNow.AddMinutes(2), Latitude = 0.002, Longitude = 0.002, Speed = 0 },
                new() { DeviceDateTime = DateTime.UtcNow.AddMinutes(3), Latitude = 0.003, Longitude = 0.003, Speed = 1 }
            };

        // Act
        var result = positions.GroupPositionsIntoTrips(_maxDistance, _maxTimeGap);

        // Assert
        result.Count().Should().Be(1);
    }

    [Test]
    public void GroupPositionsIntoTrips_LargeDatasetWithMultipleGroups_ReturnsCorrectTrips()
    {
        // Arrange
        var positions = new List<PositionVm>();
        var startTime = DateTime.UtcNow;

        // First trip
        for (int i = 0; i < 100; i++)
        {
            positions.Add(new PositionVm
            {
                DeviceDateTime = startTime.AddMinutes(i),
                Latitude = i * 0.001,
                Longitude = i * 0.001,
                Speed = i % 10
            });
        }

        // Adding a gap to create a new trip
        startTime = startTime.AddHours(2);

        // Second trip
        for (int i = 0; i < 100; i++)
        {
            positions.Add(new PositionVm
            {
                DeviceDateTime = startTime.AddMinutes(i),
                Latitude = i * 0.001,
                Longitude = i * 0.001,
                Speed = i % 10
            });
        }

        // Adding another gap to create a third trip
        startTime = startTime.AddHours(2);

        // Third trip
        for (int i = 0; i < 100; i++)
        {
            positions.Add(new PositionVm
            {
                DeviceDateTime = startTime.AddMinutes(i),
                Latitude = i * 0.001,
                Longitude = i * 0.001,
                Speed = i % 10
            });
        }

        // Act
        var result = positions.GroupPositionsIntoTrips(_maxDistance, _maxTimeGap);

        // Debugging output
        Console.WriteLine($"Total trips: {result.Count()}");
        foreach (var trip in result)
        {
            Console.WriteLine($"Trip ID: {trip.TripId}, Points: {trip.Points.Count}, Duration: {trip.Duration}, Total Distance: {trip.TotalDistance}");
        }

        // Assert
        result.Count().Should().Be(3);
    }
}
