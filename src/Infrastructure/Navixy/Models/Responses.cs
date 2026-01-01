// Copyright (c) 2025 Sergio Hernandez. All rights reserved.
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

namespace TrackHub.Router.Infrastructure.Navixy.Models;

/// <summary>
/// Response from Navixy tracker/list API
/// </summary>
internal sealed record TrackerListResponse(
    bool Success,
    IEnumerable<Tracker>? List
);

/// <summary>
/// Response from Navixy track/list API
/// </summary>
internal sealed record TrackListResponse(
    bool Success,
    IEnumerable<TrackListEntry>? List
);

/// <summary>
/// Response from Navixy track/read API
/// </summary>
internal sealed record TrackReadResponse(
    bool Success,
    bool Limit_exceeded,
    IEnumerable<TrackPoint>? List
);

/// <summary>
/// Response from Navixy user/auth API
/// </summary>
internal sealed record AuthResponse(
    bool Success,
    string? Hash
);
