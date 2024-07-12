﻿using TrackHubRouter.Application.Devices.Queries.Get;
using TrackHubRouter.Application.Devices.Queries.GetByOperator;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Web.GraphQL;

public partial class Query
{
    public async Task<IEnumerable<ExternalDeviceVm>> GetDevices([Service] ISender sender)
        => await sender.Send(new GetDevicesQuery());

    public async Task<IEnumerable<ExternalDeviceVm>> GetDevicesByOperator([Service] ISender sender, [AsParameters] GetDevicesByOperatorQuery query)
        => await sender.Send(query);
}