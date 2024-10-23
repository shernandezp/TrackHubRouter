using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHubRouter.Domain.Records;

namespace ManagerApi;

// This class represents a CredentialWriter that implements the ICredentialWriter interface.
// It is responsible for updating a token asynchronously.
public class CredentialWriter(IGraphQLClientFactory graphQLClient) : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), ICredentialWriter
{
    /// <summary>
    /// It constructs a GraphQL request with the necessary variables and sends a mutation to update the token.
    /// </summary>
    /// <param name="id">The ID of the token to update</param>
    /// <param name="credential">The credential information to update</param>
    /// <param name="token">The cancellation token</param>
    /// <returns>A boolean indicating the success of the update operation</returns>
    public async Task<bool> UpdateTokenAsync(Guid id, UpdateTokenDto credential, CancellationToken token)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    mutation($id:UUID!, $credentialId: UUID!, $refreshToken: String, $refreshTokenExpiration: DateTime, $token: String, $tokenExpiration: DateTime) {
                      updateToken(id: $id,
                            command: { credential: { credentialId: $credentialId, refreshToken: $refreshToken, refreshTokenExpiration: $refreshTokenExpiration, token: $token, tokenExpiration: $tokenExpiration } })
                    }",
            Variables = new
            {
                id,
                credentialId = credential.CredentialId,
                refreshToken = credential.RefreshToken,
                refreshTokenExpiration = credential.RefreshTokenExpiration,
                token = credential.Token,
                tokenExpiration = credential.TokenExpiration
            }
        };
        return await MutationAsync<bool>(request, token);
    }
}
