using System.Text;
using TrackHubRouter.Domain.Records;

namespace TrackHubRouter.Domain.Extensions;
public static class DeviceExtensions
{
    public static string GetIdsQueryString(this IEnumerable<DeviceDto> devices)
    {
        var stringBuilder = new StringBuilder();
        bool isFirst = true;
        foreach (var device in devices)
        {
            if (!isFirst)
            {
                stringBuilder.Append('&');
            }
            stringBuilder.Append("id=").Append(device.Identifier);
            isFirst = false;
        }
        return stringBuilder.ToString();
    }
}
