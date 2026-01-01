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

using System.Text;
using System.Text.Json;
using Ardalis.GuardClauses;
using Common.Domain.Extensions;
using TrackHubRouter.Domain.Interfaces.Manager;

namespace TrackHub.Router.Infrastructure.CommandTrack.Helpers;
// Helper class for managing tokens
internal class TokenHelper(ICredentialWriter credentialWriter)
{
    // Retrieves a token asynchronously
    public async Task<string> GetTokenAsync(HttpClient httpClient,
        CredentialTokenDto credential,
        CancellationToken token)
    {
        return string.IsNullOrEmpty(credential.Token) || IsTokenExpired(credential)
            ? await RefreshTokenAsync(httpClient, credential, token)
            : credential.Token;
    }

    // Refreshes the token asynchronously
    private async Task<string> RefreshTokenAsync(HttpClient httpClient, CredentialTokenDto credential, CancellationToken token)
    {
        Guard.Against.Null(credential.Key, message: "Credential key not found.");

        var model = new AuthenticateModel
        {
            Username = credential.Username,
            Password = credential.Password,
            UniqueId = credential.Key
        };

        var response = await httpClient.PostAsync("SecurityApi/Auth/authenticate", new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json"), token);
        response.EnsureSuccessStatusCode();

        var tokenResponseContent = await response.Content.ReadAsStringAsync(token);
        var newToken = tokenResponseContent.Deserialize<TokenResult>();

        if (newToken.Token is null)
        {
            throw new Exception("Failed to retrieve a new token");
        }

        await credentialWriter.UpdateTokenAsync(credential.CredentialId, new UpdateTokenDto(
                credential.CredentialId,
                newToken.Token,
                newToken.Expires,
                null,
                null
            ), token);

        return newToken.Token;
    }

    // Checks if the token has expired
    private static bool IsTokenExpired(CredentialTokenDto token)
        => DateTime.UtcNow >= token.TokenExpiration;
}
