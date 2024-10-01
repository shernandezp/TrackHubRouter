﻿using Common.Domain.Extensions;
using TrackHub.Router.Infrastructure.Traccar.Mappers;
using TrackHubRouter.Domain.Extensions;
using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Traccar;

// This class represents a reader for retrieving position information from Traccar.
public sealed class PositionReader(ICredentialHttpClientFactory httpClientFactory, IHttpClientService httpClientService) : TraccarReaderBase(httpClientFactory, httpClientService)
{
    /// <summary>
    /// Retrieves the position of a single device asynchronously.
    /// </summary>
    /// <param name="deviceDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The position view model of the device.</returns>
    public async Task<PositionVm> GetDevicePositionAsync(DeviceOperatorVm deviceDto, CancellationToken cancellationToken)
    {
        var url = $"api/positions?id={deviceDto.Identifier}";
        var position = await HttpClientService.GetAsync<Position>(url, cancellationToken: cancellationToken);
        return position.MapToPositionVm(deviceDto);
    }

    /// <summary>
    /// Retrieves the positions of multiple devices asynchronously.
    /// </summary>
    /// <param name="devices"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The collection of position view models of the devices.</returns>
    public async Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceOperatorVm> devices, CancellationToken cancellationToken)
    {
        //Traccar does not have a method to retrieve the last position of multiple devices at once
        //So we need to retrieve the position id from the devices first
        var url = $"api/devices?{devices.GetIdsQueryString()}";
        var result = await HttpClientService.GetAsync<IEnumerable<Device>>(url, cancellationToken: cancellationToken);
        if (result is null)
        {
            return [];
        }
        url = $"api/positions?{result.Select(x => x.PositionId).GetIdsQueryString()}";
        var positions = await HttpClientService.GetAsync<IEnumerable<Position>>(url, cancellationToken: cancellationToken);
        if (positions is null)
        {
            return [];
        }
        var devicesDictionary = devices.ToDictionary(device => device.Identifier, device => device);
        return positions.MapToPositionVm(devicesDictionary);
    }

    /// <summary>
    /// Retrieves the positions of a device within a specified time range asynchronously.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="deviceDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The collection of position view models of the device within the specified time range.</returns>
    public async Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceOperatorVm deviceDto, CancellationToken cancellationToken)
    {
        var url = $"api/positions?deviceId={deviceDto.Identifier}&from={from.ToIso8601String()}&to={to.ToIso8601String()}";
        var positions = await HttpClientService.GetAsync<IEnumerable<Position>>(url, cancellationToken: cancellationToken);
        return positions is null ? ([]) : positions.MapToPositionVm(deviceDto);
    }
}
