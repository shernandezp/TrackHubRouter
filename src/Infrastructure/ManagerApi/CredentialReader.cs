using Common.Application.Interfaces;
using Common.Domain.Constants;
using Common.Infrastructure;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Models;
using GraphQL;

namespace ManagerApi;

public class CredentialReader(IGraphQLClientFactory graphQLClient) : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), ICredentialReader
{
    public async Task<CredentialVm> GetCredentialAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    query($id: UUID!) {
                        credential(query: { id: $id }) 
                        {
                            uri
                            username
                            password
                            key
                            key2
                        }
                    }",
            Variables = new { id }
        };
        return await QueryAsync<CredentialVm>(request, cancellationToken);
    }

    public async Task<string> GetCredentialUrlAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    query($id: UUID!) {
                        credential(query: { id: $id }) 
                        {
                            uri
                        }
                    }",
            Variables = new { id }
        };
        var credential = await QueryAsync<CredentialVm>(request, cancellationToken);
        return credential.Uri;
    }

    public async Task<CredentialTokenVm> GetCredentialTokenAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    query($id: UUID!) {
                        credentialToken(query: { id: $id })
                        {
                            token
                            tokenExpiration
                            refreshToken
                            refreshTokenExpiration
                        }
                    }",
            Variables = new { id }
        };
        return await QueryAsync<CredentialTokenVm>(request, cancellationToken);
    }
}
