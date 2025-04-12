#!/usr/bin/env bash

[ "${NUGET_API_KEY}" = '' ] && echo "❌ 'NUGET_API_KEY' env var not set" && exit 1

set -eou pipefail

echo "🔍 Full release detected, building with version"
dotnet pack ./Lithium/Lithium.csproj --output nupkgs
dotnet pack ./LithiumTestHelper/LithiumTestHelper.csproj --output nupkgs

echo "📦 Publishing packages..."
dotnet nuget push ./nupkgs/*.nupkg --api-key "${NUGET_API_KEY}" --source "https://api.nuget.org/v3/index.json" --skip-duplicate
echo "✅ Packages published!"
