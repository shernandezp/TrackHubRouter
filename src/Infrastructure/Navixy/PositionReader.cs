// Copyright (c) 2025 Sergio Hernandez. All rights reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License").
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using TrackHub.Router.Infrastructure.Navixy.Mappers;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Interfaces.Operator;

namespace TrackHub.Router.Infrastructure.Navixy;

/// <summary>
/// Reader for retrieving position information from Navixy API.
/// </summary>
public sealed class PositionReader(
    ICredentialHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService)
    : NavixyReaderBase(httpClientFactory, httpClientService), IPositionReader
{
    /// <summary>
    /// Retrieves the last position of a single device asynchronously.
    /// Uses tracker/list which includes last_update with current position.
    /// </summary>
    public async Task<PositionVm> GetDevicePositionAsync(DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
    {
        var result = await HttpClientService.PostAsync<TrackerListResponse>(
            $"{BaseUrl}/v2/tracker/list", new { hash = Hash }, cancellationToken);
        
        var tracker = result?.List?.FirstOrDefault(t => t.Tracker_id == deviceDto.Identifier);
        return tracker is null
            ? throw new InvalidOperationException($"Device not found: {deviceDto.Identifier}")
            : tracker.Value.MapToPositionVm(deviceDto);
    }

    /// <summary>
    /// Retrieves the last positions of multiple devices asynchronously.
    /// </summary>
    public async Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceTransporterVm> devices, CancellationToken cancellationToken)
    {
        var result = await HttpClientService.PostAsync<TrackerListResponse>(
            $"{BaseUrl}/v2/tracker/list", new { hash = Hash }, cancellationToken);
        
        if (result?.List is null || !result.List.Any())
        {
            return [];
        }

        var devicesDictionary = devices.ToDictionary(device => device.Identifier, device => device);
        return result.List
            .Where(t => devicesDictionary.ContainsKey((int)t.Tracker_id) && t.Last_update.HasValue)
            .MapToPositionVm(devicesDictionary)
            .Distinct();
    }

    /// <summary>
    /// Retrieves the positions of a device within a specified time range asynchronously.
    /// Uses track/read endpoint to get historical position data.
    /// </summary>
    public async Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
    {
        var result = await HttpClientService.PostAsync<TrackReadResponse>(
            $"{BaseUrl}/v2/track/read",
            new
            {
                hash = Hash,
                tracker_id = deviceDto.Identifier,
                from = FormatNavixyDate(from),
                to = FormatNavixyDate(to)
            },
            cancellationToken);

        return result?.List is null ? [] : result.List.MapToPositionVm(deviceDto);
    }
}
