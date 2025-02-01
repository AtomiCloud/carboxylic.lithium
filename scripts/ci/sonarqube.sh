#!/usr/bin/env bash

[ "${ATOMI_PLATFORM}" = '' ] && echo "❌ 'ATOMI_PLATFORM' env var not set" && exit 1
[ "${ATOMI_SERVICE}" = '' ] && echo "❌ 'ATOMI_SERVICE' env var not set" && exit 1
[ "${SONAR_TOKEN}" = '' ] && echo "❌ 'SONAR_TOKEN' env var not set" && exit 1
[ "${SONAR_HOST}" = '' ] && echo "❌ 'SONAR_HOST' env var not set" && exit 1
[ "${SONAR_ORG}" = '' ] && echo "❌ 'SONAR_ORG' env var not set" && exit 1

set -eou pipefail

# export path
export PATH="$PATH:$HOME/.dotnet/tools"

echo "⬇️ Installing Dependencies..."
dotnet restore
dotnet tool restore
echo "✅ Done!"

project="AtomiCloud_${ATOMI_PLATFORM}.${ATOMI_SERVICE}"

echo "📡 Starting SonarQube..."
dotnet sonarscanner begin /k:"${project}" /o:"${SONAR_ORG}" /d:sonar.token="${SONAR_TOKEN}" /d:sonar.host.url="${SONAR_HOST}" /d:sonar.cs.dotcover.reportsPaths=dotCover.Output.html
dotnet build --no-incremental
dotnet dotcover test --dcReportType=HTML
dotnet sonarscanner end /d:sonar.token="${SONAR_TOKEN}"
echo "✅ Done!"
