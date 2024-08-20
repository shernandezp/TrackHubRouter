using Common.Application.Interfaces;
using Common.Domain.Constants;
using Common.Infrastructure;
using TrackHubRouter.Domain.Models;
using GraphQL;
using TrackHubRouter.Domain.Interfaces.Manager;

namespace ManagerApi;

// This class represents the implementation of the IOperatorReader interface
// It is responsible for reading operator data from the GraphQL service
public class OperatorReader(IGraphQLClientFactory graphQLClient) : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), IOperatorReader
{

    // Retrieves a list of operators associated with the current user
    // Parameters:
    // - cancellationToken: A cancellation token to cancel the operation if needed
    // Returns:
    // - A collection of OperatorVm objects representing the operators
    public async Task<IEnumerable<OperatorVm>> GetOperatorsAsync(CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    query {
                        operatorsByUser
                        {
                            operatorId
                            protocolTypeId
                            credential {
                                credentialId
                                uri
                                username
                                password
                                salt
                                key
                                key2
                                token
                                tokenExpiration
                                refreshToken
                                refreshTokenExpiration
                            }
                        }
                    }"
        };
        return await QueryAsync<IEnumerable<OperatorVm>>(request, cancellationToken);
    }

    public async Task<OperatorVm> GetOperatorAsync(Guid operatorId, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    query($id: UUID!) {
                        operator(query: { id: $id })
                        {
                            operatorId
                            protocolTypeId
                            credential {
                                credentialId
                                uri
                                username
                                password
                                salt
                                key
                                key2
                                token
                                tokenExpiration
                                refreshToken
                                refreshTokenExpiration
                            }
                        }
                    }",
            Variables = new { id = operatorId }
        };
        return await QueryAsync<OperatorVm>(request, cancellationToken);
    }
}
