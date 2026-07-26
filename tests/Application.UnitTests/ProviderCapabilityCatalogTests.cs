// Copyright (c) 2026 Sergio Hernandez. All rights reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License").
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using Common.Domain.Enums;
using TrackHub.Router.Application.Providers.Queries;
using TrackHub.Router.Domain.Constants;
using TrackHub.Router.Domain.Enumerators;
using TrackHub.Router.Domain.Exceptions;

namespace TrackHub.Router.Application.UnitTests.Providers;

[TestFixture]
public class ProviderCapabilityCatalogTests
{
    [Test]
    public void EveryProtocolTypeValue_HasACatalogEntry()
    {
        // The catalog is the client-facing truth for every protocol the enum can name — a new
        // enum value without a declaration would silently report ProviderCapability.None.
        var declared = ProviderCapabilityCatalog.All.Select(e => e.Key).ToHashSet();
        var missing = Enum.GetValues<ProtocolType>().Where(p => !declared.Contains(p));

        Assert.That(missing, Is.Empty);
    }

    [Test]
    public void GpsGate_DeclaresNoPositionHistory_ButRealTimePositions()
    {
        Assert.Multiple(() =>
        {
            Assert.That(ProviderCapabilityCatalog.Supports(ProtocolType.GpsGate, ProviderCapability.PositionHistory), Is.False);
            Assert.That(ProviderCapabilityCatalog.Supports(ProtocolType.GpsGate, ProviderCapability.RealTimePositions), Is.True);
        });
    }

    [Test]
    public void EnsureSupports_MissingCapability_ThrowsWithProtocolAndCapability()
    {
        var ex = Assert.Throws<ProviderCapabilityNotSupportedException>(
            () => ProviderCapabilityCatalog.EnsureSupports(ProtocolType.GpsGate, ProviderCapability.PositionHistory));

        Assert.Multiple(() =>
        {
            Assert.That(ex!.Protocol, Is.EqualTo(ProtocolType.GpsGate));
            Assert.That(ex.Capability, Is.EqualTo(ProviderCapability.PositionHistory));
            Assert.That(ex.Message, Does.Contain("GpsGate").And.Contain("provider"));
        });
    }

    [Test]
    public async Task GetProviderCapabilitiesQuery_ReturnsTheDeclaredMatrix()
    {
        var handler = new GetProviderCapabilitiesQueryHandler();

        var result = (await handler.Handle(new GetProviderCapabilitiesQuery(), CancellationToken.None)).ToList();

        var gpsGate = result.Single(r => r.ProtocolTypeId == (int)ProtocolType.GpsGate);
        var traccar = result.Single(r => r.ProtocolTypeId == (int)ProtocolType.Traccar);
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(Enum.GetValues<ProtocolType>().Length));
            Assert.That(gpsGate.Protocol, Is.EqualTo(nameof(ProtocolType.GpsGate)));
            Assert.That(gpsGate.PositionHistory, Is.False);
            Assert.That(gpsGate.RealTimePositions, Is.True);
            Assert.That(traccar.PositionHistory, Is.True);
        });
    }
}
