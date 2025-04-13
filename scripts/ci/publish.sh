#!/usr/bin/env bash

[ "${NUGET_API_KEY}" = '' ] && echo "❌ 'NUGET_API_KEY' env var not set" && exit 1
targets="$1"
[ -z "$targets" ] && echo "❌ Targets argument is required" && exit 1

set -eou pipefail

echo "🔍 Full release detected, building with version"

# Split comma-delimited targets into array
IFS=',' read -ra target_array <<<"$targets"

# Clear any existing nupkgs
mkdir -p nupkgs
rm -f ./nupkgs/*.nupkg

# Loop through targets and pack each
for target in "${target_array[@]}"; do
  echo "📦 Packing $target"
  dotnet pack "$target/$target.csproj" --output nupkgs
done

echo "📦 Publishing packages..."
dotnet nuget push ./nupkgs/*.nupkg --api-key "${NUGET_API_KEY}" --source "https://api.nuget.org/v3/index.json" --skip-duplicate
echo "✅ Packages published!"
