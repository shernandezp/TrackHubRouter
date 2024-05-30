using Common.Application.Interfaces;
using Common.Infrastructure;
using GraphQL;
using GraphQL.Client.Abstractions;

namespace TrackHubRouter.Infrastructure.Identity;
public class IdentityService(IGraphQLClient graphQLClient) : GraphQLService(graphQLClient), IIdentityService
{

    public Task<string> GetUserNameAsync(Guid userId, CancellationToken token)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    query($userId: UUID!) {
                        userName(query: { userId: $userId })
                    }",
            Variables = new { userId }
        };
        return QueryAsync<string>(request, token);
    }

    public Task<bool> AuthorizeAsync(Guid userId, string resource, string action, CancellationToken token)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    query($action: String!, $resource: String!, $userId: UUID!) {
                        authorize(query: { action: $action, resource: $resource, userId: $userId })
                    }",
            Variables = new { userId, resource, action }
        };
        return QueryAsync<bool>(request, token);
    }

    public Task<bool> IsInRoleAsync(Guid userId, string resource, string action, CancellationToken token)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    query($action: String!, $resource: String!, $userId: UUID!) {
                        isInRole(query: { action: $action, resource: $resource, userId: $userId })
                    }",
            Variables = new { userId, resource, action }
        };
        return QueryAsync<bool>(request, token);
    }
}
