#!/usr/bin/env bash

[ "${RELEASE}" = '' ] && echo "❌ 'RELEASE' env var not set" && exit 1

[ "${GITHUB_SHA}" = '' ] && echo "❌ 'GITHUB_SHA' env var not set" && exit 1
[ "${GITHUB_BRANCH}" = '' ] && echo "❌ 'GITHUB_BRANCH' env var not set" && exit 1
[ "${GITHUB_REPO_REF}" = '' ] && echo "❌ 'GITHUB_REPO_REF' env var not set" && exit 1

[ "${NUGET_API_KEY}" = '' ] && echo "❌ 'NUGET_API_KEY' env var not set" && exit 1

set -eou pipefail

SHA="$(echo "${GITHUB_SHA}" | head -c 6)"
# shellcheck disable=SC2001
BRANCH="$(echo "${GITHUB_BRANCH}" | sed 's/[._-]*$//')"
RELEASE_VERSION="${BRANCH}-${SHA}"

echo "🪵 Current Branch: $BRANCH"

if [ "${RELEASE}" == "true" ]; then
  echo "🔍 Full release detected, building with version"
  dotnet pack ./lib/lib.csproj --output nupkgs
else
  echo "🔍 Pre-release detected, building with version suffix, $RELEASE_VERSION"
  dotnet pack ./lib/lib.csproj --version-suffix "$RELEASE_VERSION" --output nupkgs
fi

echo "📦 Publishing packages..."
dotnet nuget push ./nupkgs/*.nupkg --api-key "${NUGET_API_KEY}" --source "https://api.nuget.org/v3/index.json" --skip-duplicate
echo "✅ Packages published!"
