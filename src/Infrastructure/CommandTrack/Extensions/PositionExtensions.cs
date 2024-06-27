namespace TrackHub.Router.Infrastructure.CommandTrack.Extensions;
internal static class PositionExtensions
{
    public static string GetAddress(this string address, double distanceToAddress)
        => distanceToAddress > 0 ? $"{address} ({distanceToAddress} km)" : address;
}
