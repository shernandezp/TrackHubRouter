namespace TrackHubRouter.Application.Devices.Queries.GetByOperator;

public class GetDevicesByOperatorValidator : AbstractValidator<GetDevicesByOperatorQuery>
{
    public GetDevicesByOperatorValidator()
    {
        RuleFor(x => x.OperatorId)
            .NotEmpty();
    }
}
