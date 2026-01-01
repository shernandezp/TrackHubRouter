## Componentes y Recursos Utilizados

| Componente                | Descripción                                             | Documentación                                                                 |
|---------------------------|---------------------------------------------------------|-------------------------------------------------------------------------------|
| Hot Chocolate             | Servidor GraphQL para .Net        | [Documentación Hot Chocolate](https://chillicream.com/docs/hotchocolate/v13)                           |
| GraphQL.Client            | Cliente Http para GraphQL        | [Documentación OpenIDDict](https://openiddict.com/)                           |
| Scalar.AspNetCore         | Integración de Scalar API para Net Core    | [Documentación Scalar](https://guides.scalar.com/scalar/scalar-api-references/net-integration)                    |
| .NET Core                 | Plataforma de desarrollo para aplicaciones modernas     | [Documentación .NET Core](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9/overview) |

# API de Ruteo para TrackHub

## Características Clave

La API de ruteo es un componente esencial de TrackHub, diseñado para agilizar y unificar la integración de datos provenientes de múltiples servicios de proveedores externos de GPS (denominados "Operadores"). Permite a TrackHub agregar datos de ubicación de manera consistente y confiable, independientemente del proveedor de origen, al devolver un formato estandarizado que cumple con los requisitos únicos de TrackHub.
También proporciona un endpoint REST para que aplicaciones de terceros accedan a los datos de todas las unidades.

## Funcionalidad

Basándose en las configuraciones establecidas en el panel de administración, la API de ruteo:

- Unifica y procesa los contratos de datos de cada proveedor de GPS.
- Estandariza los datos entrantes a un formato genérico y escalable.
- Permite a TrackHub integrar múltiples servicios de proveedores de manera eficiente y consistente.

### Operadores Actualmente Implementados

| Operador      | Enlace a la Documentación                           | Estado de Implementación | Probado   |
|---------------|----------------------------------------------------|--------------------------|-----------|
| CommandTrack  | [Documentación de CommandTrack](https://www.c2ls.co/home/documentacion-de-la-api/) | ✅ Implementado           | ✅ Probado |
| Flespi        | [Documentación de Flespi](https://flespi.io/docs/)       | ✅ Implementado            | ❌ No Probado |
| GeoTab        | [Documentación de GeoTab](https://developers.geotab.com/myGeotab/guides/codeBase/usingInDotnet)      | ✅ Implementado           | ❌ No Probado |
| GpsGate       | [Documentación de GpsGate](https://support.gpsgate.com/hc/en-us/articles/360016602140-REST-API-Documentation)     | ⚠️ Implementación Parcial | ❌ No Probado |
| Navixy        | [Documentación de Navixy](https://www.navixy.com/docs/navixy-api/user-api/getting-started)       | ✅ Implementado            | ❌ No Probado |
| Samsara       | [Documentación deSamsara](https://developers.samsara.com/docs/tms-integration)       | ✅ Implementado            | ❌ No Probado |
| Traccar       | [Documentación de Traccar](https://www.traccar.org/api-reference/)     | ✅ Implementado           | ✅ Probado |
| Wialon        | [Documentación de Wialon](https://help.wialon.com/en/api/user-guide)       | ✅ Implementado            | ❌ No Probado |

## Servicio de Sincronización

Para mejorar la confiabilidad, el proyecto incluye un Servicio de Sincronización que actualiza regularmente la información de los dispositivos en la base de datos local. Este servicio asegura que TrackHub muestre la última posición conocida de cada dispositivo, incluso si un proveedor externo experimenta problemas de conectividad o está temporalmente fuera de línea.

## REST API
Para facilitar la integración con terceros, TrackHub proporciona una API REST con métodos para recuperar información de las unidades. Esta API utiliza el Router API como middleware para acceder a los datos de ubicación GPS de todas las unidades.

![Image](https://github.com/shernandezp/TrackHub/blob/main/src/assets/images/api.png?raw=true)

## Futuras Actualizaciones

- **Ampliación del soporte a proveedores de GPS adicionales**: Incorporar nuevos protocolos y proveedores para ampliar la compatibilidad.

## Licencia

Este proyecto está bajo la Licencia Apache 2.0. Consulta el archivo [LICENSE](https://www.apache.org/licenses/LICENSE-2.0) para más información.