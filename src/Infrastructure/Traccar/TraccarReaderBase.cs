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
using System.Text;
using Common.Domain.Enums;
using TrackHubRouter.Domain.Interfaces;
namespace TrackHub.Router.Infrastructure.Traccar;

// This class represents the base class for Traccar readers.
public abstract class TraccarReaderBase(
    ICredentialHttpClientFactory httpClientFactory, 
    IHttpClientService httpClientService)
{
    protected IHttpClientService HttpClientService { get; } = httpClientService;

    public ProtocolType Protocol => ProtocolType.Traccar;

    // Converts the credential to a base64-encoded string.
    private static string GetCredentialString(CredentialTokenDto credential)
    {
        var credentials = $"{credential.Username}:{credential.Password}";
        return Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
    }

    // Initializes the Traccar reader with the provided credential.
    public virtual void Init(CredentialTokenDto credential, CancellationToken cancellationToken = default)
    {
        var httpClient = httpClientFactory.CreateClientAsync(credential, cancellationToken);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetCredentialString(credential));
        HttpClientService.Init(httpClient, $"{ProtocolType.Traccar}");
    }
}
