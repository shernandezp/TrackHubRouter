using System.Text.Json;
using Common.Application.Interfaces;
using GraphQL;
using GraphQL.Client.Abstractions;

namespace TrackHubRouter.Infrastructure.Identity;
public class IdentityService(IGraphQLClient graphQLClient) : IIdentityService
{

    private async Task<T> QueryAsync<T>(GraphQLRequest request, CancellationToken token)//move this two to a base class or an extension method
    {

        var response = await graphQLClient.SendQueryAsync<object>(request, token);

        if (response == null || response.Data == null || (response.Errors != null && response.Errors.Length > 0))
        {
            throw new Exception("GraphQL query execution error.");
        }

        var dataString = response.Data.ToString();
        return string.IsNullOrEmpty(dataString)
            ? throw new Exception("Data string is null or empty.")
            : ExtractFirstPropertyValue<T>(dataString);
    }

    private T ExtractFirstPropertyValue<T>(string json)
    {
        var dataObject = JsonDocument.Parse(json);
        var property = dataObject.RootElement.EnumerateObject().FirstOrDefault();
        if (property.Value.ValueKind != JsonValueKind.Null)
        {
            var propertyJson = property.Value.GetRawText();
            var propertyValue = JsonSerializer.Deserialize<T>(propertyJson);
            if (propertyValue != null)
                return propertyValue;
        }
        throw new Exception("response is null or empty.");
    }

    public Task<string> GetUserNameAsync(Guid userId, CancellationToken token)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    query($userId: UUID!) {
                        userName(userId: $userId)
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
                    query($userId: UUID!, $resource: String!, $action: String!) {
                        authorize(userId: $userId, resource: $resource, action: $action)
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
