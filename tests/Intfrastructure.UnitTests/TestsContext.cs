using TrackHubRouter.Domain.Models;

namespace Application.UnitTests;

public abstract class TestsContext
{
    protected CredentialTokenVm TestCredentialTokenVm { get; } = new()
    {
        Salt = "yhhidBsASfV6Hh1VLYHD+suDa7cjDgLtgOfKVcOl1i8=",
        Username = "TJeYWbqXtz7bn+j3VolgkVY2LXze37E80K3wC2zNWpSHG6IEuaYEh6UD+2DAdie7XKP3kk3i5pvQc/hDxNwfZQ==",
        Password = "TJeYWbqXtz7bn+j3VolgkVY2LXze37E80K3wC2zNWpSHG6IEuaYEh6UD+2DAdie7XKP3kk3i5pvQc/hDxNwfZQ==",
        Key = "TJeYWbqXtz7bn+j3VolgkVY2LXze37E80K3wC2zNWpSHG6IEuaYEh6UD+2DAdie7XKP3kk3i5pvQc/hDxNwfZQ==",
        Uri = "https://www.example.com/"
    };
}
