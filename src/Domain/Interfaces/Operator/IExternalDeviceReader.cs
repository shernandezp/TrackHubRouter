﻿namespace TrackHubRouter.Domain.Interfaces.Operator;

public interface IExternalDeviceReader
{
    ProtocolType Protocol { get; }
    Task Init(CredentialTokenDto credential, CancellationToken cancellationToken = default);
    Task<DeviceVm> GetDeviceAsync(DeviceOperatorVm deviceDto, CancellationToken cancellationToken);
    Task<IEnumerable<DeviceVm>> GetDevicesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<DeviceVm>> GetDevicesAsync(IEnumerable<DeviceOperatorVm> devices, CancellationToken cancellationToken);
}
