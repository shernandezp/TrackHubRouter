using Common.Infrastructure;

namespace TrackHubRouter.Infrastructure.Entities;
public sealed class Credential(string uri, string username, string password, string key, string key2, string salt, string? token, DateTime? tokenExpiration, string? refreshToken, Guid operatorId) : BaseAuditableEntity
{
    private Operator? _operator;

    public Guid CredentialId { get; set; } = Guid.NewGuid();
    public string Uri { get; set; } = uri;
    public string Username { get; set; } = username;
    public string Password { get; set; } = password;
    public string Key { get; set; } = key;
    public string Key2 { get; set; } = key2;
    public string Salt { get; set; } = salt;
    public string? Token { get; set; } = token;
    public DateTime? TokenExpiration { get; set; } = tokenExpiration;
    public string? RefreshToken { get; set; } = refreshToken;
    public Guid OperatorId { get; set; } = operatorId;

    public Operator Operator
    {
        get => _operator ?? throw new InvalidOperationException("Operator is not loaded");
        set => _operator = value;
    }
}
