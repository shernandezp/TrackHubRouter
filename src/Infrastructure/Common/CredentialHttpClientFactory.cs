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

using TrackHubRouter.Domain.Records;

namespace TrackHub.Router.Infrastructure.Common;

// This class represents a factory for creating HttpClient instances with credentials.
public sealed class CredentialHttpClientFactory(IHttpClientFactory httpClientFactory) : ICredentialHttpClientFactory
{
    // Creates a new HttpClient instance with the specified credential and cancellation token.
    // If the credential has a valid URI, sets the BaseAddress and Timeout properties of the HttpClient.
    // Otherwise, throws an InvalidOperationException.
    public HttpClient CreateClientAsync(CredentialTokenDto credential, CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(credential.CredentialId.ToString());
        if (!string.IsNullOrEmpty(credential.Uri))
        {
            httpClient.BaseAddress = new Uri(credential.Uri);
            httpClient.Timeout = TimeSpan.FromSeconds(30);
        }
        else
        {
            throw new InvalidOperationException($"Base URL for client '{credential.CredentialId}' not initialized.");
        }
        return httpClient;
    }
}
