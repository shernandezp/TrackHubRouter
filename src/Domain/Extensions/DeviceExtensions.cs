using System.Text;

namespace TrackHubRouter.Domain.Extensions;
public static class DeviceExtensions
{
    public static string GetIdsQueryString(this IEnumerable<DeviceTransporterVm> devices)
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

    public static string GetIdsQueryString(this IEnumerable<int> ids)
    {
        var stringBuilder = new StringBuilder();
        bool isFirst = true;
        foreach (var id in ids.Where(x => x != 0))
        {
            if (!isFirst)
            {
                stringBuilder.Append('&');
            }
            stringBuilder.Append("id=").Append(id);
            isFirst = false;
        }
        return stringBuilder.ToString();
    }

}
