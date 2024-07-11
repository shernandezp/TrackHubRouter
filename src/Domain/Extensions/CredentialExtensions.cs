using Common.Domain.Extensions;

namespace TrackHubRouter.Domain.Extensions;
public static class CredentialExtensions
{
    public static CredentialTokenDto Decrypt(this CredentialTokenVm credential, string key)
    {
        var salt = Convert.FromBase64String(credential.Salt);
        return new CredentialTokenDto
        (
            credential.CredentialId,
            credential.Uri,
            credential.Username.DecryptData(key, salt),
            credential.Password.DecryptData(key, salt),
            credential.Key?.DecryptData(key, salt),
            credential.Key2?.DecryptData(key, salt),
            credential.Token?.DecryptData(key, salt),
            credential.TokenExpiration,
            credential.RefreshToken?.DecryptData(key, salt),
            credential.RefreshTokenExpiration
        );
    }
}
