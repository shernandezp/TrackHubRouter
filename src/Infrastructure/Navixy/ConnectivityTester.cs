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

using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Navixy;

/// <summary>
/// Connectivity tester for Navixy API.
/// Tests connection by authenticating and fetching tracker list.
/// </summary>
public sealed class ConnectivityTester(
    ICredentialHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService)
    : NavixyReaderBase(httpClientFactory, httpClientService), IConnectivityTester
{
    /// <summary>
    /// Tests connectivity by attempting to authenticate with the Navixy API.
    /// </summary>
    public async Task Ping(CredentialTokenDto credential, CancellationToken cancellationToken)
    {
        await Init(credential, cancellationToken);
        // Make a simple API call to verify the session hash is valid
        await HttpClientService.PostAsync<TrackerListResponse>(
            $"{BaseUrl}/v2/tracker/list", new { hash = Hash }, cancellationToken);
    }
}
