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
using System.Text.Json;
using Common.Domain.Enums;
using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Wialon;

/// <summary>
/// Base class for Wialon readers providing common functionality for API communication.
/// Wialon uses token-based authentication via the token/login endpoint.
/// </summary>
public class WialonReaderBase
{
    private readonly ICredentialHttpClientFactory _httpClientFactory;
    private string _sid = string.Empty;
    private string _baseUrl = string.Empty;

    protected IHttpClientService HttpClientService { get; }

    public ProtocolType Protocol => ProtocolType.Wialon;

    protected WialonReaderBase(ICredentialHttpClientFactory httpClientFactory, IHttpClientService httpClientService)
    {
        HttpClientService = httpClientService;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Initializes the Wialon reader with the provided credential.
    /// Authenticates using the token and obtains a session ID (sid).
    /// </summary>
    public virtual async Task Init(CredentialTokenDto credential, CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClientAsync(credential, cancellationToken);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _baseUrl = credential.Uri.TrimEnd('/');

        HttpClientService.Init(httpClient, $"{ProtocolType.Wialon}");

        // Login using token to get session ID
        var loginUrl = $"{_baseUrl}/wialon/ajax.html?svc=token/login";
        var loginParams = new { token = credential.Token };
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "params", JsonSerializer.Serialize(loginParams) }
        });

        var loginResponse = await HttpClientService.PostAsync<LoginResponse>(loginUrl, content, cancellationToken);
        _sid = string.IsNullOrEmpty(loginResponse?.Eid)
            ? throw new InvalidOperationException("Failed to obtain session ID from Wialon")
            : loginResponse.Eid;
    }

    /// <summary>
    /// Makes a POST request to the Wialon API.
    /// </summary>
    protected async Task<T?> PostAsync<T>(string svc, object parameters, CancellationToken cancellationToken)
    {
        var url = $"{_baseUrl}/wialon/ajax.html?svc={svc}&sid={_sid}";
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "params", JsonSerializer.Serialize(parameters) }
        });

        return await HttpClientService.PostAsync<T>(url, content, cancellationToken);
    }
}
