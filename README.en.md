## Components and Resources

| Component                | Description                                           | Documentation                                                                 |
|--------------------------|-------------------------------------------------------|-------------------------------------------------------------------------------|
| Hot Chocolate            | GraphQL server for .NET                               | [Hot Chocolate Documentation](https://chillicream.com/docs/hotchocolate/v13)  |
| GraphQL.Client           | HTTP client for GraphQL                               | [OpenIDDict Documentation](https://openiddict.com/)                           |
| .NET Core                | Development platform for modern applications          | [.NET Core Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9/overview) |

# Router API for TrackHub

## Key Features

The Routing API is an essential component of TrackHub, designed to streamline and unify data integration from multiple external GPS provider services (referred to as "Operators"). It enables TrackHub to aggregate location data consistently and reliably, regardless of the source provider, by returning a standardized format that meets TrackHub’s unique requirements.

## Functionality

Based on configurations set in the administration panel, the Routing API:

- Unifies and processes data contracts from each GPS provider.
- Standardizes incoming data to a scalable, generic format.
- Enables TrackHub to integrate multiple provider services efficiently and consistently.

### Currently Implemented Operators

| Operator      | Documentation Link                                    | Implementation Status   | Tested    |
|---------------|-------------------------------------------------------|--------------------------|-----------|
| CommandTrack  | [CommandTrack Documentation](https://www.c2ls.co/home/documentacion-de-la-api/) | ✅ Implemented            | ✅ Tested |
| GeoTab        | [GeoTab Documentation](https://developers.geotab.com/myGeotab/guides/codeBase/usingInDotnet)       | ✅ Implemented            | ❌ Not Tested |
| GpsGate       | [GpsGate Documentation](https://support.gpsgate.com/hc/en-us/articles/360016602140-REST-API-Documentation)      | ⚠️ Partially Implemented  | ❌ Not Tested |
| Traccar       | [Traccar Documentation](https://www.traccar.org/api-reference/)      | ✅ Implemented            | ✅ Tested |

## Synchronization Service

To enhance reliability, the project includes a Synchronization Service that regularly updates device information in the local database. This service ensures that TrackHub displays the latest known position for each device, even if an external provider experiences connectivity issues or is temporarily offline.

## Future Updates

- **Expanding support for additional GPS providers**: Incorporate new protocols and providers to broaden compatibility.
- **Enabling third-party access to TrackHub through REST endpoints**: Provide secure access to TrackHub data for third-party applications.
- **Geofencing capabilities**: Enable the service to reference device positions based on defined geofences, allowing for more precise monitoring and location-based alerts.

## License

This project is licensed under the Apache 2.0 License. See the [LICENSE file](https://www.apache.org/licenses/LICENSE-2.0) for more information.