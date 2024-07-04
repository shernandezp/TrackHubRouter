using Common.Application.Interfaces;
using Common.Domain.Constants;
using Common.Infrastructure;
using GraphQL;
using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHubRouter.Domain.Records;

namespace ManagerApi;

public class CredentialWriter(IGraphQLClientFactory graphQLClient) : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), ICredentialWriter
{
    public async Task<bool> UpdateTokenAsync(Guid id, UpdateTokenDto credential, CancellationToken token)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                mutation($id:UUID!, $active: Boolean!, $userId: UUID!, $username: String!) {
                  updateToken(id: $id,
                        command: { credential: { credentialId: $credentialId, refreshToken: $refreshToken, refreshTokenExpiration: $refreshTokenExpiration, token: $token, tokenExpiration: $tokenExpiration } })
                }",
            Variables = new
            {
                id,
                credential.CredentialId,
                credential.RefreshToken,
                credential.RefreshTokenExpiration,
                credential.Token,
                credential.TokenExpiration
            }
        };
        var result = await MutationAsync<bool>(request, token);
        return result;
    }
}
