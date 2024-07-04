using Common.Application.Interfaces;
using Common.Domain.Constants;
using Common.Infrastructure;
using TrackHubRouter.Domain.Models;
using GraphQL;
using TrackHubRouter.Domain.Interfaces.Manager;

namespace ManagerApi;

public class CredentialReader(IGraphQLClientFactory graphQLClient) 
    : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), ICredentialReader
{

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
        var credential = await QueryAsync<CredentialTokenVm>(request, cancellationToken);
        return credential.Uri;
    }

    public async Task<TokenVm> GetTokenAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    query($id: UUID!) {
                        token(query: { id: $id })
                        {
                            token
                            tokenExpiration
                            refreshToken
                            refreshTokenExpiration
                        }
                    }",
            Variables = new { id }
        };
        return await QueryAsync<TokenVm>(request, cancellationToken);
    }
}
