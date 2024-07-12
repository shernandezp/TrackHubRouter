using Common.Application.Interfaces;
using Common.Domain.Constants;
using Common.Infrastructure;
using GraphQL;
using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHubRouter.Domain.Records;

namespace ManagerApi;

// This class represents a CredentialWriter that implements the ICredentialWriter interface.
// It is responsible for updating a token asynchronously.
public class CredentialWriter(IGraphQLClientFactory graphQLClient) : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), ICredentialWriter
{
    // This method updates a token asynchronously.
    // It takes in the id of the token, the credential information, and a cancellation token.
    // It constructs a GraphQL request with the necessary variables and sends a mutation to update the token.
    // It returns a boolean indicating the success of the update operation.
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
