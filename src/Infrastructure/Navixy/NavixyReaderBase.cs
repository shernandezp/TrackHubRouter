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

using System.Net.Http.Headers;
using Common.Domain.Enums;
using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Navixy;

/// <summary>
/// Base class for Navixy readers providing common functionality for API communication.
/// Navixy uses session hash authentication obtained via user/auth endpoint.
/// </summary>
public class NavixyReaderBase
{
    private readonly ICredentialHttpClientFactory _httpClientFactory;

    protected IHttpClientService HttpClientService { get; }

    public ProtocolType Protocol => ProtocolType.Navixy;

    // Navixy date format: yyyy-MM-dd HH:mm:ss
    protected const string NavixyDateFormat = "yyyy-MM-dd HH:mm:ss";

    /// <summary>
    /// Gets the current session hash.
    /// </summary>
    protected string Hash { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the base URL for API calls.
    /// </summary>
    protected string BaseUrl { get; private set; } = string.Empty;

    /// <summary>
    /// Formats a DateTimeOffset to Navixy date format.
    /// </summary>
    protected static string FormatNavixyDate(DateTimeOffset date)
        => date.ToString(NavixyDateFormat);

    protected NavixyReaderBase(ICredentialHttpClientFactory httpClientFactory, IHttpClientService httpClientService)
    {
        HttpClientService = httpClientService;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Initializes the Navixy reader with the provided credential.
    /// Authenticates using user/auth to obtain a session hash.
    /// </summary>
    public virtual async Task Init(CredentialTokenDto credential, CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClientAsync(credential, cancellationToken);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        BaseUrl = credential.Uri.TrimEnd('/');

        HttpClientService.Init(httpClient, $"{ProtocolType.Navixy}");

        // Authenticate to get session hash
        var authUrl = $"{BaseUrl}/v2/user/auth";
        var authResponse = await HttpClientService.PostAsync<AuthResponse>(authUrl, 
            new { login = credential.Username, password = credential.Password }, cancellationToken);

        Hash = string.IsNullOrEmpty(authResponse?.Hash)
            ? throw new InvalidOperationException("Failed to obtain session hash from Navixy")
            : authResponse.Hash;
    }

    /// <summary>
    /// Parses a Navixy date string to DateTimeOffset.
    /// </summary>
    protected static DateTimeOffset ParseNavixyDate(string dateStr)
        => DateTimeOffset.TryParseExact(dateStr, NavixyDateFormat, null, 
            System.Globalization.DateTimeStyles.AssumeUniversal, out var result)
            ? result
            : DateTimeOffset.MinValue;
}
